using UtpTypes.UtpMessageContext;
using UtpTypes.Services;
using ActionCodes = UtpTypes.Actions.ActionCode;

namespace UtpTypes.Handlers;

// ReSharper disable once UnusedMember.Global
public class PingHandler(IServiceLocator serviceLocator) : UtpHandlerBase(serviceLocator)
{
    public override short ActionCode => (short)ActionCodes.Ping;

    public override Task HandleAsync(UtpContext ctx)
    {
        throw new NotImplementedException();
    }
}
