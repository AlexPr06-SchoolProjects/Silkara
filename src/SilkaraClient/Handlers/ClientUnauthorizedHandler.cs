using Microsoft.Extensions.Logging;
using UtpTypes.Actions;
using UtpTypes.Handlers;
using UtpTypes.Services;
using UtpTypes.UtpMessageContext;

namespace SilkaraClient.Handlers;

public class ClientUnauthorizedHandler : UtpHandlerBase
{
    public override short ActionCode => (short)ServerCode.Unauthorized;

    public override Task HandleAsync(UtpContext ctx)
    {
        // TODO: Handle the unauthorized request
        var logger = GlobalServiceLocator.Instance.GetRequiredService<ILogger>();
        logger.LogInformation("Unauthorized request received.");
        return Task.CompletedTask;
    }
}