using UtpTypes.Handlers;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Routers;

public class UtpRouter
{
    private readonly Dictionary<short, IUtpHandler> _handlers;

    public UtpRouter(IEnumerable<IUtpHandler> handlers)
    {
        _handlers = handlers.ToDictionary(h => h.ActionCode);
    }

    public async ValueTask RouteAsync(UtpContext ctx)
    {
        if (_handlers.TryGetValue(ctx.ActionCode, out var handler)) 
        {
            await handler.HandleAsync(ctx);
        }
    }
}
