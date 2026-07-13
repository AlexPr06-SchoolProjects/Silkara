using UTP.Payload;
using UtpTypes.Services;

namespace UtpTypes.UtpMessageContext;

public class UtpContext : IUtpContext
{
    public IServiceLocator? ServiceLocator { get; set; }
    public CancellationToken CancellationToken { get; init; }
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
        ActionCode = actionCode;
        Headers = headers;
        Payload = payload;
    }

    public UtpContext(
        CancellationToken ct,
        short actionCode,
        Dictionary<string, string> headers,
        IPayload payload) : this(actionCode, headers, payload)
    {
        CancellationToken = ct;
    }

    public UtpContext(
        IServiceLocator serviceLocator,
        short actionCode,
        Dictionary<string, string> headers,
        IPayload payload) : this(actionCode, headers, payload)
    {
        ServiceLocator = serviceLocator;
    }

    public UtpContext(
        CancellationToken ct,
        IServiceLocator serviceLocator,
        short actionCode,
        Dictionary<string, string> headers,
        IPayload payload) : this(actionCode, headers, payload)
    {
        CancellationToken = ct;
        ServiceLocator = serviceLocator;
    }

    public void Dispose()
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (Payload is IDisposable disposablePayload)
            disposablePayload.Dispose();
    }
}
