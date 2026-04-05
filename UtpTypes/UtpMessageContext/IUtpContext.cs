using UTP.Payload;

namespace UtpTypes.UtpMessageContext;

public interface IUtpContext
{
    Guid ClientId { get; }
    short ActionCode { get; }
    Dictionary<string, string> Headers { get; }
    IPayload Payload { get; }
}