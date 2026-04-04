using UTP.Payload;

namespace UtpTypes.UtpMessageContext;

public class UtpContext(Guid clientId, short actionCode, Dictionary<string, string> headers, IPayload payload)
{
    public Guid ClientId { get; } = clientId;
    public short ActionCode { get; } = actionCode;
    public Dictionary<string, string> Headers { get; } = headers;
    public IPayload Payload { get; } = payload;
}
