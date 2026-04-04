using Microsoft.Extensions.Logging;
using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

internal class LoggingMiddleware(ILogger logger) : IUtpMiddleware
{
    public async ValueTask InvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        logger.LogInformation(
        "Client {ClientId} ActionCode {ActionCode}",
        ctx.ClientId,
        ctx.ActionCode);

        await next(ctx);
    }
}
