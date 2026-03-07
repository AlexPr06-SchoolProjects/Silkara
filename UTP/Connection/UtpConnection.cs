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
    private readonly CancellationTokenSource _cts = new();

    private const int MINIMUM_BUFFER_SIZE = UtpConstants.UtpConnectionConstants.MINIMUM_BUFFER_SIZE;

    public UtpConnection(Socket socket)
    {
        var options = new PipeOptions(useSynchronizationContext: false);
        _socket = socket;
        _receivePipe = new Pipe(options);
        _sendPipe = new Pipe(options);
        _ = ReceiveAsync(_socket, _receivePipe.Writer, _cts.Token);
        _ = ReadAsync(_socket, _sendPipe.Reader, _cts.Token);
    }

    public PipeReader Reader => _receivePipe.Reader;

    public PipeWriter Writer => _sendPipe.Writer;

    private async Task ReceiveAsync(Socket socket, PipeWriter writer, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                Memory<byte> memory = writer.GetMemory(MINIMUM_BUFFER_SIZE);
                int bytesRead = await socket.ReceiveAsync(memory, SocketFlags.None, ct);
                if (bytesRead == 0) break;
                writer.Advance(bytesRead);
                FlushResult result = await writer.FlushAsync(ct);
                if (result.IsCompleted) break;
            }
        }
        catch (OperationCanceledException) { }
        catch { }
        finally { await writer.CompleteAsync(); }
    }

    private async Task ReadAsync(Socket socket, PipeReader reader, CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                ReadResult result = await reader.ReadAsync(ct);
                ReadOnlySequence<byte> buffer = result.Buffer;

                foreach(ReadOnlyMemory<byte> segment in buffer)
                    await socket.SendAsync(segment, SocketFlags.None, ct);

                reader.AdvanceTo(buffer.End);
                if (result.IsCompleted) break;
            }
        }
        catch (OperationCanceledException) { }
        catch { }
        finally { await reader.CompleteAsync(); }
    }

    public async ValueTask DisposeAsync()
    {
        _cts.Cancel();
        _socket.Close();
        _socket.Dispose();
        _cts.Dispose();
        await Task.CompletedTask;
    }
}


