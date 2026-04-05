using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware.MiddlewareConcretes;

public interface IUtpMiddleware
{
    ValueTask InvokeAsync(UtpContext ctx, UtpDelegate next);
}
