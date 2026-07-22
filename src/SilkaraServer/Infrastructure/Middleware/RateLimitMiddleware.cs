using UtpTypes.Middleware.Delegates;
using UtpTypes.Services;
using UtpTypes.UtpMessageContext;
using UtpTypes.Middleware;
using SilkaraServer.Domain.Id;
using SilkaraServer.Infrastructure.Settings;
using UtpTypes.Actions;
using SilkaraServer.Infrastructure.Middleware.Extensions;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace SilkaraServer.Infrastructure.Middleware;

internal class RateLimitMiddleware(IServiceLocator serviceLocator) : UtpMiddlewareBase(serviceLocator)
{
    private readonly int _maxRequestsPerMinute = GlobalServerSettings.MaxRequestsPerMinute;
    protected override async ValueTask OnInvokeAsync(UtpContext ctx, UtpDelegate next)
    {
        var idManager = ctx.ServiceLocator?.GetRequiredService<IClientIdManager>();
        var utpClient = ctx.ServiceLocator?.GetRequiredService<UtpServerClient>();
        var logger = GlobalServiceLocator.Instance.GetRequiredService<ILogger>();
        var redisDb = ctx.ServiceLocator?.GetRequiredService<IDatabase>();

        if (idManager?.ClientId is not null)
        {
            if (redisDb is null)
            {
                logger.LogWarning("RateLimitMiddleware: Redis database is null.");
                await utpClient.NotifyUser(
                    (short)ServerCode.Error,
                    "Inner server error. Redis database is not defined."
                );
                return;
            }
            if (await IsThrottled(idManager.ClientId, redisDb))
            {
                await utpClient.NotifyUser(
                    (short)ServerCode.RateLimitExceeded,
                    "You have exceeded the maximum number of requests per minute.");
                return;
            }
        }
        else
        {
            await utpClient.NotifyUser(
                (short)ServerCode.ClientIdNotFound,
                "You don't have a client id. That could be a server issue. " +
                "Please try to reconnect.");
            return;
        }

        await next(ctx);
    }

    private async Task<bool> IsThrottled(Guid clientId, IDatabase redisDb)
    {
        string key = $"ratelimit:{clientId}:{DateTime.UtcNow:yyyyMMddHHmm}";

        long count = await redisDb.StringIncrementAsync(key);

        if (count == 1)
        {
            await redisDb.KeyExpireAsync(key, TimeSpan.FromSeconds(65));
        }

        return count > _maxRequestsPerMinute;
    }
}
