using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;
using UtpTypes.Services;
using SilkaraServer.Services;
using UtpTypes.Middleware;
using SilkaraServer.Client.Managers.Id;
using SilkaraServer.Client;
using UTP.UtpMessage;
using UtpTypes.PayloadTypes.Server;
using UtpTypes.Actions;
using UtpTypes.UtpClientType;
using SilkaraServer.Services.StateServicesConcretes;
using Microsoft.Extensions.Logging;

namespace SilkaraServer.Middleware;

public class StateValidationMiddleware(IServiceLocator serviceLocator) 
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
            if (utpClient is not null)
                await NotifyUserAboutUnauthorization(utpClient);
            logger.LogInformation("State manager is null.");
            return;
        }

        if (!stateManager.CurrentState.CanExecute(ctx.ActionCode))
        {
            if (utpClient is not null)
                await NotifyUserAboutUnauthorization(utpClient);
            logger.LogInformation($"Client {idManager?.ClientId} tried action {ctx.ActionCode} while in {stateManager.CurrentState.GetType().Name}");
            return;
            // throw new UnauthorizedAccessException(
            //     $"Client {idManager?.ClientId} tried action {ctx.ActionCode} while in {stateManager.CurrentState.GetType().Name}");
        }

        logger.LogInformation($"Client {idManager?.ClientId} tried action {ctx.ActionCode} while in {stateManager.CurrentState.GetType().Name}");
    
        await next(ctx);
    }

    private async Task NotifyUserAboutUnauthorization(UtpClient client)
        => await client.SendMessageAsync(
                new UtpMessage<ClientUnauthorizedPayload>(
                    (short)ServerCode.Error, 
                    new Dictionary<string, string>(), 
                    new ClientUnauthorizedPayload()));

}