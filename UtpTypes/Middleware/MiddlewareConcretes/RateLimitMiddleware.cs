using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

internal class RateLimitMiddleware : IUtpMiddleware
{
    public async ValueTask InvokeAsync(UtpContext ctx, UtpDelegate next)
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
