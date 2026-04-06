using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

public abstract class UtpMiddlewareBase : IUtpMiddleware
{
    public ValueTask InvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        if (ctx.CancellationToken.IsCancellationRequested)
            return ValueTask.CompletedTask;
        
        return OnInvokeAsync(ctx, next);
    }

    protected abstract ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next);
}