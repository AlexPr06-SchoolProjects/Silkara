using UtpTypes.UtpMessageContext;

namespace UtpTypes.Handlers;

public class PingHandler : IUtpHandler
{
    public short ActionCode => throw new NotImplementedException();

    public Task HandleAsync(UtpContext ctx)
    {
        throw new NotImplementedException();
    }
}
