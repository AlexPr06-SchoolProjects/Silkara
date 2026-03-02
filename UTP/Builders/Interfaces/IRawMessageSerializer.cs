using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders.Interfaces;

internal interface IMessageSerializer<TPayload>
    where TPayload : IPayload
{
    public IMessageSerializer<TPayload> SerializeActionCode(UtpMessage<TPayload> utpMessage);
    public IMessageSerializer<TPayload> SerializeHeaders(UtpMessage<TPayload> utpMessage);
    public IMessageSerializer<TPayload> SerializePayload(UtpMessage<TPayload> utpMessage);
    public RawUtpMessage Build();
    public void Reset();
}
