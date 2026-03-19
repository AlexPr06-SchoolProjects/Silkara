using UtpTypes.Middleware.Delegates;
using UtpTypes.Middleware.MiddlewaresConcrete;
using UtpTypes.Routers;

namespace UtpTypes.Pipelines;

internal class UtpPipeline
{
    private readonly List<IUtpMiddleware> _middlewares = new();
    private readonly UtpRouter _router;

    public UtpPipeline(UtpRouter router)
    {
        _router = router;
    }

    public void Use(IUtpMiddleware middleware)
    {
        _middlewares.Add(middleware);
    }

    public UtpDelegate Build()
    {
        UtpDelegate pipeline = ctx => _router.RouteAsync(ctx);

        foreach (var middleware in _middlewares)
        {
            var next = pipeline;
            pipeline = ctx => middleware.InvokeAsync(ctx, next);
        }

        return pipeline;
    }
}
