using System.Text.Json;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders;

internal static class MessageSerializeBuilder
{
    public static IMessageSerializer<TPayload> For<TPayload>() 
        where TPayload : IPayload
    {
        return new MessageSerializeBuilder<TPayload>();
    }
}


internal class MessageSerializeBuilder<TPayload> : IMessageSerializer<TPayload>
    where TPayload : IPayload
{
    private RawUtpMessage _serialized;

    public MessageSerializeBuilder()
    {
        _serialized = new RawUtpMessage();
    }

    public RawUtpMessage Build()
        => _serialized;

    public void Reset()
        => _serialized.Clear();
   

    public IMessageSerializer<TPayload> SerializeActionCode(UtpMessage<TPayload> utpMessage)
    {
        _serialized.ActionCode = utpMessage.ActionCode;
        return this;
    }

    public IMessageSerializer<TPayload> SerializeHeaders(UtpMessage<TPayload> utpMessage)
    {
       var headers = utpMessage.Headers;
       byte[] serializedHeaders = JsonSerializer.SerializeToUtf8Bytes(headers);
        _serialized.Headers = serializedHeaders;
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
