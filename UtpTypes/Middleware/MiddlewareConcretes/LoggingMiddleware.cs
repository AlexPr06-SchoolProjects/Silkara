using Microsoft.Extensions.Logging;
using UtpTypes.Middleware.Delegates;
using UtpTypes.Middleware.MiddlewaresConcrete;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

internal class LoggingMiddleware : IUtpMiddleware
{
    private readonly ILogger _logger;

    public LoggingMiddleware(ILogger logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        _logger.LogInformation(
            "Client {ClientId} ActionCode {ActionCode}",
            ctx.ClientId,
            ctx.ActionCode);

        await next(ctx);
    }
}
