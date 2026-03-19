using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewaresConcrete;

internal interface IUtpMiddleware
{
    Task InvokeAsync(UtpContext ctx, UtpDelegate next);
}
