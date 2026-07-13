using UtpTypes.Actions;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Handlers;

// ReSharper disable once UnusedMember.Global
public class PingHandler : UtpHandlerBase
{
    public override short ActionCode => (short)MessageCode.Ping;

    public override Task HandleAsync(UtpContext ctx)
    {
        throw new NotImplementedException();
    }
}
