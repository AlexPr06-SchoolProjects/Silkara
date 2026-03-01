using UTP.Payload;

namespace UTP.UtpMessage.Interfaces;

public interface IUtpMessage<TPayload> : 
    IBaseUtpMessage<short, IDictionary<string, string>, TPayload?>
        where TPayload : IPayload
{ }
