using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

internal interface IUtpMiddleware
{
    ValueTask InvokeAsync(UtpContext ctx, UtpDelegate next);
}
