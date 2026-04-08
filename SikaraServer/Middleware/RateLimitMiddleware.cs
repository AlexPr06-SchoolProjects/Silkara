using UtpTypes.Middleware.Delegates;
using UtpTypes.Services;
using UtpTypes.UtpMessageContext;
using UtpTypes.Middleware;
using SilkaraServer.Client.Managers.Id;

namespace SilkaraServer.Middleware;

public class RateLimitMiddleware(IServiceLocator serviceLocator) : UtpMiddlewareBase(serviceLocator)
{
    protected override async ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        var idManager = ctx.ServiceLocator?.GetRequiredService<IClientIdManager>();
        if (idManager?.ClientId is not null && TooManyRequests(idManager.ClientId))
            return;

        await next(ctx);
    }

    private bool TooManyRequests(Guid clientId)
    {
        // TODO: implement rate limit
        return false;
    }
}
