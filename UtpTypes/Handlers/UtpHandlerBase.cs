using UtpTypes.UtpMessageContext;
using UtpTypes.Services;

namespace UtpTypes.Handlers;

public abstract class UtpHandlerBase(IServiceLocator serviceLocator) : IUtpHandler
{
    protected readonly IServiceLocator ServiceLocator = serviceLocator;

    public abstract short ActionCode { get; }
    public abstract Task HandleAsync(UtpContext ctx);
} 