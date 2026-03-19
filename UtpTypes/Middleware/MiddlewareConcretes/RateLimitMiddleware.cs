using UtpTypes.Middleware.Delegates;
using UtpTypes.Middleware.MiddlewaresConcrete;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

internal class RateLimitMiddleware : IUtpMiddleware
{
    public async Task InvokeAsync(UtpContext ctx, UtpDelegate next)
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
