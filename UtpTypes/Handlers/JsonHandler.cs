
using UtpTypes.UtpMessageContext;
using UtpTypes.Services;
using ActionCodes = UtpTypes.Actions.ActionCode;
using Microsoft.Extensions.Logging;

namespace UtpTypes.Handlers;

// ReSharper disable once UnusedMember.Global
public class JsonHandler(IServiceLocator globalServiceLocator) : UtpHandlerBase(globalServiceLocator)
{
    public override short ActionCode => (short)ActionCodes.Json;

    public override Task HandleAsync(UtpContext ctx)
    {
        var logger = GlobalServiceLocator.Instance.GetRequiredService<ILogger>();
        logger.LogInformation("JsonHandler: IT'S A JSON!");
        return Task.CompletedTask;
    }
}
