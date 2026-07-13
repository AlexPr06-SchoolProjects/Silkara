using UtpTypes.Dispatchers;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Routers;

public class UtpRouter 
{
    public async ValueTask RouteAsync(UtpContext ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        var handler = HandlersDispatcher.Instance.TryGetHandler(ctx.ActionCode);
        if (handler is not null)
        {
            await handler.HandleAsync(ctx);
        }
    }
}
