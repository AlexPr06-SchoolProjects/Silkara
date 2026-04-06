
using UtpTypes.UtpMessageContext;
using UtpTypes.Services;
using ActionCodes = UtpTypes.Actions.ActionCode;
using Microsoft.Extensions.Logging;
using UtpTypes.UtpClientType;
using UtpTypes.PayloadTypes;
using UTP.UtpMessage;

namespace UtpTypes.Handlers;

// ReSharper disable once UnusedMember.Global
public class JsonHandler : UtpHandlerBase
{
    public override short ActionCode => (short)ActionCodes.Json;
    public override async Task HandleAsync(UtpContext ctx)
    {
        var logger = GlobalServiceLocator.Instance.GetRequiredService<ILogger>();

        // HANDLING LOGIC GOES HERE 
        logger.LogInformation("🎉 Сообщение получено!");
        logger.LogInformation($"Результат: ActionCode: {ctx.ActionCode}");
        logger.LogInformation($"Результат: ClientId: {ctx.ClientId}");
        logger.LogInformation($"Результат: Headers: {ctx.Headers}");
        logger.LogInformation($"Результат: Payload: {ctx.Payload}");
        // HANDLING LOGIC GOES HERE

        // Example response logic
        if (ctx.ServiceLocator is not null)
        {
            var utpClient = ctx.ServiceLocator.GetRequiredService<UtpClient>();
            ctx.Headers.Clear();
            ctx.Headers["Response"] = "Pong";
            await utpClient.SendMessageAsync(
                new UtpMessage<JsonPayload>(ctx.ActionCode, ctx.Headers, new JsonPayload(12, "Answer from server")), 
                ct: ctx.CancellationToken);
        }
        logger.LogInformation("Response sent.");

        // Dispose of the context to free resources
        DisposeContext(ctx);

        return;
    }
}
