using UTP.Payload;

namespace UTP.UtpMessage.Interfaces;

public interface IUtpMessage<TPayload> : 
    IBaseUtpMessage<short, int, IDictionary<string, string>, Stream>
        where TPayload : IPayload
{ }
