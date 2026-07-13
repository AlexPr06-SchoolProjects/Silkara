using UTP.Payload;
using UtpTypes.Services;

namespace UtpTypes.UtpMessageContext;

public interface IUtpContext : IDisposable
{
    public IServiceLocator? ServiceLocator { get; }
    public CancellationToken CancellationToken { get; }
    short ActionCode { get; }
    Dictionary<string, string> Headers { get; }
    IPayload Payload { get; }
}