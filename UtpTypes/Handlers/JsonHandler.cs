using UtpTypes.UtpMessageContext;
using ActionCodes = UtpTypes.Actions.ActionCode;

namespace UtpTypes.Handlers;

public class JsonHandler : IUtpHandler
{
    public short ActionCode => (short)ActionCodes.Json;

    public Task HandleAsync(UtpContext ctx)
    {
        Console.WriteLine("JsonHandler is processing the message. IT'S A JSON!");
        return Task.CompletedTask;
    }
}
