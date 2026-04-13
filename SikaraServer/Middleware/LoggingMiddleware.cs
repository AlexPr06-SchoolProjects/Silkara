using Microsoft.Extensions.Logging;
using UtpTypes.Services;
using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;
using UtpTypes.Middleware;
using SilkaraServer.Client.Managers.Id;

namespace SilkaraServer.Middleware;

internal class LoggingMiddleware(IServiceLocator serviceLocator) : UtpMiddlewareBase(serviceLocator)
{
    protected override async ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next)
    {

        var logger = GlobalServiceLocator.Instance.GetRequiredService<ILogger>();
        var idManager = ctx.ServiceLocator?.GetRequiredService<IClientIdManager>();
        logger.LogInformation(
            "LoggingMiddleware: Client {ClientId} ActionCode {ActionCode}",
            idManager?.ClientId,
            ctx.ActionCode);

        await next(ctx);
    }
}
