using UTP.Payload;

namespace UtpTypes.UtpMessageContext;

public class UtpContext : IUtpContext
{
    public Guid ClientId { get; }
    public short ActionCode { get; }
    public Dictionary<string, string> Headers { get; }
    public IPayload Payload { get; }

    public UtpContext(
        short actionCode,
        Dictionary<string, string> headers,
        IPayload payload)
    {
        ClientId = Guid.Empty;
        ActionCode = actionCode;
        Headers = headers;
        Payload = payload;
    }

    public UtpContext(
        Guid clientId,
        short actionCode,
        Dictionary<string, string> headers,
        IPayload payload) : this(actionCode, headers, payload)
    {
        ClientId = clientId;
    }

}
