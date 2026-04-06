using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Text.Json;
using UTP.Connection;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage;
using UTP.Exceptions;

namespace UTP;

public class UtpEngine : IAsyncDisposable
{
    private readonly UtpConnection _connection;
    private readonly PipeWriter _writer;
    private readonly PipeReader _reader;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

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

            if (TryParseBeforePayload(ref buffer, out short actionCode, out var headers, out int payloadLength, out var consumedPos))
            {
                _reader.AdvanceTo(consumedPos);
                return (actionCode, headers, payloadLength);
            }

            _reader.AdvanceTo(buffer.Start, buffer.End);

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

            if (TryParsePayload(ref buffer, payloadLen, message, out var consumedPos))
            {
                _reader.AdvanceTo(consumedPos);
                if (message.PayloadStream != null)
                {
                    message.DisposePayloadStream();
                }
                return message;
            }

            _reader.AdvanceTo(buffer.Start, buffer.End);

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

        Span<byte> span = _writer.GetSpan(headerBlockSize);

        int offset = 0;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), dataSizeAfterLengthField);
        offset += UtpConstants.Sizes.MessageLen;

        BinaryPrimitives.WriteInt16BigEndian(span.Slice(offset), utpMessage.ActionCode);
        offset += UtpConstants.Sizes.ActionCodeLen;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), serializedHeaders.Length);
        offset += UtpConstants.Sizes.HeadersLen;

        serializedHeaders.CopyTo(span.Slice(offset));
        _writer.Advance(headerBlockSize);

        if (utpMessage.PayloadStream != null && utpMessage.PayloadStream != Stream.Null)
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
        out int payloadLen,
        out SequencePosition consumedPos
        )
    {
        actionCode = 0;
        payloadLen = 0;
        headers = null!;

        var reader = new SequenceReader<byte>(buffer);
        consumedPos = reader.Position;

        if (!reader.TryReadBigEndian(out int packetSize)) return false;
        //if (reader.Remaining < packetSize) return false;

        if (packetSize <= 0)
            throw new InvalidDataException("Packet too large");

        if (!reader.TryReadBigEndian(out short actionCodeRetrieved)) return false;
        if (!reader.TryReadBigEndian(out int headersLen)) return false;

        if (headersLen < 0 || headersLen > packetSize)
            throw new InvalidDataException("Invalid headers length");

        if (reader.Remaining < headersLen) return false; // !!! to CHECK!

        ReadOnlySequence<byte> headersData = buffer.Slice(reader.Position, headersLen);
        reader.Advance(headersLen);
        var jsonReader = new Utf8JsonReader(headersData);
        var headersRetrieved = JsonSerializer.Deserialize<Dictionary<string, string>>(ref jsonReader);

        actionCode = actionCodeRetrieved;

        headers = headersRetrieved ?? new Dictionary<string, string>();
        payloadLen = packetSize - (UtpConstants.Sizes.ActionCodeLen + UtpConstants.Sizes.HeadersLen + headersLen);
        consumedPos = reader.Position;
        return true;
    }

    private bool TryParsePayload<TPayload>(
       ref ReadOnlySequence<byte> buffer,
       int payloadLen,
       UtpMessage<TPayload> message,
       out SequencePosition consumedPos
       )
         where TPayload : IPayload
    {
        var reader = new SequenceReader<byte>(buffer);
        consumedPos = reader.Position;

        if (payloadLen == 0)
        {
            message.SetPayload(default);
            return true;
        }  

        if (payloadLen < 0)
            throw new InvalidDataException();

        if (reader.Remaining < payloadLen)
            return false;

        ReadOnlySequence<byte> payloadData = buffer.Slice(reader.Position, payloadLen);

        try
        {
            var payloadReader = new Utf8JsonReader(payloadData);
            TPayload? payload = JsonSerializer.Deserialize<TPayload>(ref payloadReader, _jsonOptions);
            message.SetPayloadWithoutStream(payload);
            reader.Advance(payloadLen);

            consumedPos = reader.Position;
            message.SetPayloadLenToHeaders((int)payloadReader.BytesConsumed);

            return true;
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Invalid payload JSON : {ex.Message}");
        }
        catch (Exception ex) {
            throw new InvalidDataException($"ERROR: {ex.Message}"); 
        }
    }

    private void CheckForExceptions(ReadResult result, ReadOnlySequence<byte> buffer)
    {
        if (!result.IsCompleted) return;
        if (buffer.IsEmpty)
            throw new RemotePeerDisconnectedException();
        throw new ConnectionClosedPrematurelyException(
            $"Connection closed prematurely. Remaining bytes in buffer: {buffer.Length}");
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
} 
