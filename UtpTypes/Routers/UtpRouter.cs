using UtpTypes.Dispatchers;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Routers;

public class UtpRouter
{
    public async ValueTask RouteAsync(UtpContext ctx)
    {
        ctx.CancellationToken.ThrowIfCancellationRequested();

        if (
            HandlersDispatcher.TryGetHandler(ctx.ActionCode, out var handler) 
            && handler is not null) 
        {
            await handler.HandleAsync(ctx);
        }
    }
}
