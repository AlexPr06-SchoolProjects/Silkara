using System.Buffers;
using System.Buffers.Binary;
using System.IO.Pipelines;
using System.Text.Json;
using UTP.Connection;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP;

public class UtpEngine(UtpConnection connection)
{
    public async Task<UtpMessage<TPayload>> ReceiveMessageAsync<TPayload>(CancellationToken ct = default)
        where TPayload : IPayload
    {
        PipeReader reader = connection.Reader;

        while (true)
        {
            ReadResult result = await reader.ReadAsync(ct);
            ReadOnlySequence<byte> buffer = result.Buffer;

            if (TryParseMessage<TPayload>(ref buffer, out var message))
            {
                reader.AdvanceTo(buffer.Start);
                return message;
            }

            reader.AdvanceTo(buffer.Start, buffer.End);

            if (result.IsCompleted)
            {
                throw new InvalidOperationException("Соединение разорвано до получения полного сообщения.");
            }
        }
    }

    public async Task SendMessageAsync<TPayload>(UtpMessage<TPayload> utpMessage, CancellationToken ct = default)
        where TPayload : IPayload
    {
        PipeWriter writer = connection.Writer;

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

        Memory<byte> memory = writer.GetMemory(headerBlockSize);
        Span<byte> span = memory.Span;

        int offset = 0;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), dataSizeAfterLengthField);
        offset += UtpConstants.Sizes.MessageLen;

        BinaryPrimitives.WriteInt16BigEndian(span.Slice(offset), utpMessage.ActionCode);
        offset += UtpConstants.Sizes.ActionCodeLen;

        BinaryPrimitives.WriteInt32BigEndian(span.Slice(offset), serializedHeaders.Length);
        offset += UtpConstants.Sizes.HeadersLen;

        serializedHeaders.CopyTo(span.Slice(offset));
        writer.Advance(headerBlockSize);

        if (utpMessage.PayloadStream != null)
        {
            if (utpMessage.PayloadStream.CanSeek) 
                utpMessage.PayloadStream.Position = 0;
            await utpMessage.PayloadStream.CopyToAsync(writer, ct);
        }

        await writer.FlushAsync(ct);
    }


    private bool TryParseMessage<TPayload>(ref ReadOnlySequence<byte> buffer, out UtpMessage<TPayload> message)
    where TPayload : IPayload
    {
        message = null!;
        var reader = new SequenceReader<byte>(buffer);

        if (!reader.TryReadBigEndian(out int packetSize)) return false;
        if (reader.Remaining < packetSize) return false;
        if (!reader.TryReadBigEndian(out short actionCode)) return false;
        if (!reader.TryReadBigEndian(out int headersLen)) return false;

        ReadOnlySequence<byte> headersData = buffer.Slice(reader.Position, headersLen);
        reader.Advance(headersLen);
        var jsonReader = new Utf8JsonReader(headersData);
        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(ref jsonReader);

        int payloadLen = packetSize - (UtpConstants.Sizes.ActionCodeLen + UtpConstants.Sizes.HeadersLen + headersLen);
        message = new UtpMessage<TPayload>(actionCode, headers);

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
        return true;
    }
} 
