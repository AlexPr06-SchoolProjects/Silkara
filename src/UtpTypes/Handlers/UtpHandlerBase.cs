using UtpTypes.UtpMessageContext;

namespace UtpTypes.Handlers;

public abstract class UtpHandlerBase : IUtpHandler
{
    public abstract short ActionCode { get; }
    public abstract Task HandleAsync(UtpContext ctx);

    public virtual void DisposeContext(UtpContext ctx)
    {
        ctx.Dispose();
    }
}
