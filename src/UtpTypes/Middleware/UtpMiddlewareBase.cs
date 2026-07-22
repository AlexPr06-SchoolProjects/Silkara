using UtpTypes.Middleware.Delegates;
using UtpTypes.Services;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware;

public abstract class UtpMiddlewareBase : IUtpMiddleware
{
    protected readonly IServiceLocator ServiceLocator;
    protected UtpMiddlewareBase(IServiceLocator serviceLocator)
    {
        ServiceLocator = serviceLocator;
    }

    public ValueTask InvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        if (ctx.CancellationToken.IsCancellationRequested)
            return ValueTask.CompletedTask;

        return OnInvokeAsync(ctx, next);
    }

    protected abstract ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next);
}
