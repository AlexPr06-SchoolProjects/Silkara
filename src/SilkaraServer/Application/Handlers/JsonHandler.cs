using Microsoft.Extensions.Logging;
using UTP.UtpMessage;
using UtpTypes.Actions;
using UtpTypes.Handlers;
using UtpTypes.PayloadTypes;
using UtpTypes.Services;
using UtpTypes.UtpMessageContext;
using SilkaraServer.Infrastructure;

namespace SilkaraServer.Application.Handlers;

// ReSharper disable once UnusedMember.Global
internal class JsonHandler : UtpHandlerBase
{
    public override short ActionCode => (short)MessageCode.Json;
    public override async Task HandleAsync(UtpContext ctx)
    {
        var logger = GlobalServiceLocator.Instance.GetRequiredService<ILogger>();

        // HANDLING LOGIC GOES HERE 
        logger.LogInformation("🎉 Сообщение получено!");
        logger.LogInformation($"Результат: ActionCode: {ctx.ActionCode} Headers: {ctx.Headers} Payload: {ctx.Payload}");
        // HANDLING LOGIC GOES HERE

        // Example response logic
        if (ctx.ServiceLocator is not null)
        {
            var utpClient = ctx.ServiceLocator.GetRequiredService<UtpServerClient>();
            ctx.Headers.Clear();
            ctx.Headers["Response"] = "Pong";
            await utpClient.SendMessageAsync(
                new UtpMessage<JsonPayload>(ctx.ActionCode, ctx.Headers, new JsonPayload(12, "Answer from server")), 
                ct: ctx.CancellationToken);
        }
        logger.LogInformation("Response sent.");

        // Dispose of the context to free resources
        DisposeContext(ctx);
    }
}
