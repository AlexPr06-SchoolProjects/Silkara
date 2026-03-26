using UTP.Payload;

namespace UtpTypes.UtpMessageContext;

public class UtpContext
{
    public Guid ClientId { get; }
    public short ActionCode { get; }
    public IPayload Payload { get; }
    public Dictionary<string, string> Headers { get; }

    public UtpContext(Guid cLientId, short actionCode, Dictionary<string, string> headers, IPayload payload)
    {
        ClientId = cLientId;
        ActionCode = actionCode;
        Headers = headers;
        Payload = payload;
    }
}
