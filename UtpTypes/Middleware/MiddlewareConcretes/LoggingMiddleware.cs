using Microsoft.Extensions.Logging;
using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

public class LoggingMiddleware(ILogger logger) : UtpMiddlewareBase
{
    protected override async ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        logger.LogInformation(
        "Client {ClientId} ActionCode {ActionCode}",
        ctx.ClientId,
        ctx.ActionCode);

        await next(ctx);
    }
}
