using UtpTypes.UtpMessageContext;
using ActionCodes = UtpTypes.Actions.ActionCode;

namespace UtpTypes.Handlers;

// ReSharper disable once UnusedMember.Global
public class PingHandler : IUtpHandler
{
    public short ActionCode => (short)ActionCodes.Ping;

    public Task HandleAsync(UtpContext ctx)
    {
        throw new NotImplementedException();
    }
}
