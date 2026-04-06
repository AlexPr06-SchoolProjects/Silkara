using UtpTypes.Middleware.Delegates;
using UtpTypes.Services;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

public class RateLimitMiddleware(IServiceLocator serviceLocator) : UtpMiddlewareBase(serviceLocator)
{
    protected override async ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        if (TooManyRequests(ctx.ClientId))
            return;

        await next(ctx);
    }

    private bool TooManyRequests(Guid clientId)
    {
        return false;
    }
}
