using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;
using UtpTypes.Services;
using UtpTypes.Middleware;
using SilkaraServer.Domain.Id;
using UtpTypes.Actions;
using SilkaraServer.Domain.Services.StateServicesConcretes;
using Microsoft.Extensions.Logging;
using SilkaraServer.Infrastructure.Middleware.Extensions;

namespace SilkaraServer.Infrastructure.Middleware;

internal class StateValidationMiddleware(IServiceLocator serviceLocator)
    : UtpMiddlewareBase(serviceLocator)
{
    protected override async ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        var stateManager = ctx.ServiceLocator?.GetRequiredService<ClientStateService>();
        var idManager = ctx.ServiceLocator?.GetRequiredService<IClientIdManager>();
        var utpClient = ctx.ServiceLocator?.GetRequiredService<UtpServerClient>();
        var logger = GlobalServiceLocator.Instance.GetRequiredService<ILogger>();

        if (stateManager is null)
        {
            await utpClient.NotifyUser(
                (short)ServerCode.Unauthorized,
                "You are not authorized to perform this action.");
            logger.LogInformation("State manager is null.");
            return;
        }

        if (!stateManager.CurrentState.CanExecute(ctx.ActionCode))
        {
            await utpClient.NotifyUser(
                (short)ServerCode.Unauthorized,
                "You are not authorized to perform this action.");
            logger.LogInformation($"Client {idManager?.ClientId} tried action {ctx.ActionCode} " +
                                  $"while in {stateManager.CurrentState.GetType().Name}");
            return;
        }

        logger.LogInformation($"Client {idManager?.ClientId} tried action {ctx.ActionCode} " +
                              $"while in {stateManager.CurrentState.GetType().Name}");

        await next(ctx);
    }
}
