using System.Text.Json;
using UTP.Builders.Interfaces;
using UTP.Constants;
using UTP.Helpers;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders;

internal static class MessageSerializeBuilder
{
    public static IMessageSerializer<TPayload> For<TPayload>() 
        where TPayload : IPayload
    {
        return new MessageSerializer<TPayload>();
    }
}


internal sealed class MessageSerializer<TPayload> : IMessageSerializer<TPayload>
    where TPayload : IPayload
{
    private readonly RawUtpMessage _serialized;

    public MessageSerializer()
    {
        _serialized = new RawUtpMessage();
    }

    public RawUtpMessage Build()
        => _serialized;

    public void Reset()
        => _serialized.Clear();
   

    public IMessageSerializer<TPayload> SerializeActionCode(UtpMessage<TPayload> utpMessage)
    {
        Span<byte> actionCodeSpan = stackalloc byte[UtpConstants.Sizes.ActionCodeLen];
        BinaryHelper.WriteToSpan(utpMessage.ActionCode, actionCodeSpan);
        _serialized.ActionCode = actionCodeSpan.ToArray();

        return this;
    }

    public IMessageSerializer<TPayload> SerializeHeaders(UtpMessage<TPayload> utpMessage)
    {
       var headers = utpMessage.Headers;
       byte[] serializedHeaders = JsonSerializer.SerializeToUtf8Bytes(headers);
        _serialized.Headers = serializedHeaders;

        Span<byte> headersLenSpan = stackalloc byte[UtpConstants.Sizes.HeadersLen];

        BinaryHelper.WriteToSpan(serializedHeaders.Length, headersLenSpan);
        _serialized.HeadersLen = headersLenSpan.ToArray();

        return this;
    }

    public IMessageSerializer<TPayload> SerializePayload(UtpMessage<TPayload> utpMessage)
    {

        if (utpMessage.Payload is null)
        {
            _serialized.Payload = Array.Empty<byte>();
            return this;
        }

        byte[] payloadBytes = JsonSerializer.SerializeToUtf8Bytes(utpMessage.Payload);
        _serialized.Payload = payloadBytes;
        return this;
    }
}
