using Microsoft.Extensions.Logging;
using UtpTypes.Services;
using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

public class LoggingMiddleware(IServiceLocator serviceLocator) : UtpMiddlewareBase(serviceLocator)
{
    protected override async ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        var logger = ServiceLocator.GetRequiredService<ILogger>();
        logger.LogInformation(
            "LoggingMiddleware: Client {ClientId} ActionCode {ActionCode}",
            ctx.ClientId,
            ctx.ActionCode);

        await next(ctx);
    }
}
