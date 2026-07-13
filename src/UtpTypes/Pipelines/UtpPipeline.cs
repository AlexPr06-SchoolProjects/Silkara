using UtpTypes.Middleware.Delegates;
using UtpTypes.Middleware;
using UtpTypes.Routers;

namespace UtpTypes.Pipelines;

public class UtpPipeline(UtpRouter router)
{
    private readonly List<IUtpMiddleware> _middlewares = new();

    public void Use(IUtpMiddleware middleware)
    {
        _middlewares.Add(middleware);
    }

    public UtpDelegate Build()
    {
        UtpDelegate pipeline = router.RouteAsync;

        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            var middleware = _middlewares[i];
            var next = pipeline;
            pipeline = ctx => middleware.InvokeAsync(ctx, next);
        }

        return pipeline;
    }
}
