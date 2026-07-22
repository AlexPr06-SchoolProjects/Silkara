using UTP.Payload;

namespace UTP.UtpMessage.Interfaces;

public interface IUtpMessage<TPayload> : IUtpMessage
    where TPayload : IPayload
{
    public TPayload? Payload { get; set; }
}
