using UTP.Payload;

namespace UTP.UtpMessage.Interfaces;

public interface IUtpMessage<TPayload> : 
    IBaseUtpMessage<short, IReadOnlyDictionary<string, string>, TPayload?>
        where TPayload : IPayload
{ }
