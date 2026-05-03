using Microsoft.Extensions.Logging;
using SilkaraServer.Handlers;
using UtpTypes.Dispatchers;
using UtpTypes.Services;
using SilkaraServer.Client.Factories;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace SilkaraServer.Server;

internal sealed class GlobalServerSetuper(IServiceProvider serviceProvider)
{
    public void Setup(ILogger logger)
    {
        // Perform any global setup for the server here
        // For example, you can register global services, configure logging, etc.

        
        // Register handlers
        GlobalServiceLocator.Instance.Register(logger);

        //TEST:
        HandlersDispatcher.AddHandler(new LoginHandler());
        HandlersDispatcher.AddHandler(new JsonHandler());
        //TEST:
        ClientFactory.Setup(serviceProvider.GetRequiredService<IDatabase>());
    }
}