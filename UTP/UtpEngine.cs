using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Text.Json;
using UTP.Connection;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP;

public class UtpEngine : IAsyncDisposable
{
    private readonly UtpConnection _connection;
    private readonly PipeWriter _writer;
    private readonly PipeReader _reader;

    public UtpEngine(UtpConnection connection)
    {
        _connection = connection;
        _reader = _connection.Reader;
        _writer = _connection.Writer;
    }

    public async Task<(short ActionCode, Dictionary<string, string> headers, int payloadLength)> ReceiveBeforePayloadAsync(CancellationToken ct = default)
    {
        while (true)
        {
            ReadResult result = await _reader.ReadAsync(ct);
            ReadOnlySequence<byte> buffer = result.Buffer;

            SequencePosition consumed = buffer.Start;
            SequencePosition examined = buffer.End;

            if (TryParseBeforePayload(ref buffer, out short actionCode, out var headers, out int payloadLength))
            {
                _reader.AdvanceTo(consumed);
                return (actionCode, headers, payloadLength);
            }

            _reader.AdvanceTo(consumed, examined);

            CheckForExceptions(result, buffer);
        }
    }
    public async Task<UtpMessage<TPayload>> ReceivePayloadAsync<TPayload>(
        int payloadLen, 
        UtpMessage<TPayload> message, 
        CancellationToken ct = default
        )
        where TPayload : IPayload
    {
        while (true)
        {
            ReadResult result = await _reader.ReadAsync(ct);
            ReadOnlySequence<byte> buffer = result.Buffer;

            SequencePosition consumed = buffer.Start;
            SequencePosition examined = buffer.End;

            if (TryParsePayload<TPayload>(ref buffer, payloadLen, message))
            {
                _reader.AdvanceTo(examined);
                return message;
            }

            _reader.AdvanceTo(consumed, examined);

            CheckForExceptions(result, buffer);
        }
    }

    public async Task SendMessageAsync<TPayload>(UtpMessage<TPayload> utpMessage, CancellationToken ct = default)
        where TPayload : IPayload
    {
        byte[] serializedHeaders = JsonSerializer.SerializeToUtf8Bytes(utpMessage.Headers);
        int payloadLen = (int)(utpMessage.PayloadStream?.Length ?? 0);


        int dataSizeAfterLengthField = UtpConstants.Sizes.ActionCodeLen +
                                   UtpConstants.Sizes.HeadersLen +
                                   serializedHeaders.Length +
                                   payloadLen;

        int headerBlockSize = UtpConstants.Sizes.MessageLen +
                              UtpConstants.Sizes.ActionCodeLen +
                              UtpConstants.Sizes.HeadersLen +
                              serializedHeaders.Length;

        Memory<byte> memory = _writer.GetMemory(headerBlockSize);
        Span<byte> span = memory.Span;

        int offset = 0;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), dataSizeAfterLengthField);
        offset += UtpConstants.Sizes.MessageLen;

        BinaryPrimitives.WriteInt16BigEndian(span.Slice(offset), utpMessage.ActionCode);
        offset += UtpConstants.Sizes.ActionCodeLen;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), serializedHeaders.Length);
        offset += UtpConstants.Sizes.HeadersLen;

        serializedHeaders.CopyTo(span.Slice(offset));
        _writer.Advance(headerBlockSize);

        if (utpMessage.PayloadStream != null)
        {
            if (utpMessage.PayloadStream.CanSeek) 
                utpMessage.PayloadStream.Position = 0;
            await utpMessage.PayloadStream.CopyToAsync(_writer, ct);
        }

        await _writer.FlushAsync(ct);
    }

    private bool TryParseBeforePayload(
        ref ReadOnlySequence<byte> buffer,
        out short actionCode,
        out Dictionary<string, string> headers,
        out int payloadLen
        )
    {
        actionCode = default;
        payloadLen = default;
        headers = null!;

        var reader = new SequenceReader<byte>(buffer);

        if (!reader.TryReadBigEndian(out int packetSize)) return false;
        if (reader.Remaining < packetSize) return false;
        if (!reader.TryReadBigEndian(out short actionCodeRetrieved)) return false;
        if (!reader.TryReadBigEndian(out int headersLen)) return false;
        if (reader.Remaining < headersLen) return false;

        ReadOnlySequence<byte> headersData = buffer.Slice(reader.Position, headersLen);
        reader.Advance(headersLen);
        var jsonReader = new Utf8JsonReader(headersData);
        var headersRetrieved = JsonSerializer.Deserialize<Dictionary<string, string>>(ref jsonReader);
        headers = headersRetrieved ?? new Dictionary<string, string>();

        payloadLen = packetSize - (UtpConstants.Sizes.ActionCodeLen + UtpConstants.Sizes.HeadersLen + headersLen);
        return true;
    }

    private bool TryParsePayload<TPayload>(
       ref ReadOnlySequence<byte> buffer,
       int payloadLen,
       UtpMessage<TPayload> message)
   where TPayload : IPayload
    {
        var reader = new SequenceReader<byte>(buffer);

        if (payloadLen > 0)
        {
            ReadOnlySequence<byte> payloadData = buffer.Slice(reader.Position, payloadLen);
            var payloadReader = new Utf8JsonReader(payloadData);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            TPayload? payload = JsonSerializer.Deserialize<TPayload>(ref payloadReader, options);
            message.SetPayload(payload);
            reader.Advance(payloadLen);
        }
        buffer = buffer.Slice(reader.Position);

        if (payloadLen == 0)
            return true;

        if (reader.Remaining < payloadLen)
            return false;

        return false;
    }

    private void CheckForExceptions(ReadResult result, ReadOnlySequence<byte> buffer)
    {
        if (result.IsCompleted)
        {
            if (buffer.Length == 0)
                throw new EndOfStreamException("Client disconnected");

            throw new InvalidOperationException(
                "Connection closed before message was fully received");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
} 
