using UtpTypes.UtpMessageContext;
using UtpTypes.Services;

namespace UtpTypes.Handlers;

public abstract class UtpHandlerBase : IUtpHandler
{
    public UtpHandlerBase() { }
    public abstract short ActionCode { get; }
    public abstract Task HandleAsync(UtpContext ctx);

    public virtual void DisposeContext(UtpContext ctx)
    {
        ctx.Dispose();
    }
} 