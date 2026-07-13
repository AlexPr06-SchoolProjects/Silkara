using UtpTypes.Middleware.Delegates;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Middleware;

public interface IUtpMiddleware
{
    ValueTask InvokeAsync(UtpContext ctx, UtpDelegate next);
}
