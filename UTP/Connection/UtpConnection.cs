using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using UTP.Constants;

namespace UTP.Connection;

public class UtpConnection : IAsyncDisposable
{
    private readonly Socket _socket;
    private readonly Pipe _receivePipe;
    private readonly Pipe _sendPipe;
    private readonly CancellationTokenSource _internalCts;
    private const int MinimumBufferSize = UtpConstants.UtpConnectionConstants.MinimumBufferSize;

    public UtpConnection(Socket socket, CancellationToken externalToken = default)
    {
        var options = new PipeOptions(useSynchronizationContext: false);
        _socket = socket;
        _receivePipe = new Pipe(options);
        _sendPipe = new Pipe(options);
        _internalCts = new CancellationTokenSource();
        var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(_internalCts.Token, externalToken).Token;
        _ = FillPipeAsync(_socket, _receivePipe.Writer, linkedToken);
        _ = ReadPipeAsync(_socket, _sendPipe.Reader, linkedToken);
    }

    public PipeReader Reader => _receivePipe.Reader;

    public PipeWriter Writer => _sendPipe.Writer;

    private async Task FillPipeAsync(Socket socket, PipeWriter writer, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                Memory<byte> memory = writer.GetMemory(MinimumBufferSize);
                int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None, ct);
                if (bytesRead == 0) break; // Socket closed
                writer.Advance(bytesRead);
                FlushResult result = await writer.FlushAsync(ct);
                if (result.IsCompleted) break;
            }
        }
        catch (OperationCanceledException ) { }
        finally { await writer.CompleteAsync(); }
    }

    private async Task ReadPipeAsync(Socket socket, PipeReader reader, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                ReadResult result = await reader.ReadAsync(ct);
                ReadOnlySequence<byte> buffer = result.Buffer;

                if (buffer.IsEmpty)
                {
                    if (result.IsCompleted) break;
                    continue;
                }

                foreach (ReadOnlyMemory<byte> segment in buffer)
                {
                    int sent = 0;
                    while (sent < segment.Length)
                    {
                        int lastSent = await socket.SendAsync(segment.Slice(sent), SocketFlags.None, ct);
                        if (lastSent <= 0) break;
                        sent += lastSent;
                    }
                }   

                reader.AdvanceTo(buffer.End);
                if (result.IsCompleted || result.IsCanceled) break;
            }
        }
        catch (OperationCanceledException) { }
        finally { await reader.CompleteAsync(); }
    }

    public async ValueTask DisposeAsync()
    {
        await _internalCts.CancelAsync();
        try
        {
            if (_socket.Connected) _socket.Shutdown(SocketShutdown.Both);
        } 
        catch { /* Socket is already closed */ }
        _socket.Dispose();
        await _receivePipe.Writer.CompleteAsync();
        await _receivePipe.Reader.CompleteAsync();
        await _sendPipe.Writer.CompleteAsync();
        await _sendPipe.Reader.CompleteAsync();
        _internalCts.Dispose();
    }
}


