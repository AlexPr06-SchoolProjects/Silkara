using UTP.Payload;
using UtpTypes.Services;

namespace UtpTypes.UtpMessageContext;

public class UtpContext : IUtpContext
{
    public IServiceLocator? ServiceLocator { get; set; }
    public CancellationToken CancellationToken { get; init; }
    public Guid ClientId { get; }
    public short ActionCode { get; }
    public Dictionary<string, string> Headers { get; }
    public IPayload Payload { get; }

    public UtpContext(
        short actionCode,
        Dictionary<string, string> headers,
        IPayload payload)
    {
        ServiceLocator = null;
        CancellationToken = CancellationToken.None;
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

    public UtpContext(
        CancellationToken ct,
        Guid clientId,
        short actionCode,
        Dictionary<string, string> headers,
        IPayload payload) : this(clientId, actionCode, headers, payload)
    {
        CancellationToken = ct;
    }

    public void Dispose()
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (Payload is IDisposable disposablePayload)
            disposablePayload.Dispose();
    }
}
