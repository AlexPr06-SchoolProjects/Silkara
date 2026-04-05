using UtpTypes.Middleware.Delegates;
using UtpTypes.Middleware.MiddlewareConcretes;
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

        foreach (var middleware in _middlewares)
        {
            var next = pipeline;
            pipeline = ctx => middleware.InvokeAsync(ctx, next);
        }

        return pipeline;
    }
}
