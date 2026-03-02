using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders.Interfaces;

internal interface IMessageDeserializer<TPayload> : IDisposable
    where TPayload : IPayload
{
    public IMessageDeserializer<TPayload> PrepareBuffer();
    public IMessageDeserializer<TPayload> DeserializeActionCode();
    public IMessageDeserializer<TPayload> DeserializeHeaders();
    public IMessageDeserializer<TPayload> DeserializePayload();
    public UtpMessage<TPayload> Build();
    public void Reset();
}
