using Microsoft.Extensions.Logging;
using SilkaraServer.Handlers;
using UtpTypes.Dispatchers;
using UtpTypes.Services;

namespace SilkaraServer.Server;

internal class GlobalServerSetuper
{
    public static GlobalServerSetuper Instance { get; } = new GlobalServerSetuper();

    private GlobalServerSetuper() { }
    public void Setup(ILogger logger)
    {
        // Perform any global setup for the server here
        // For example, you can register global services, configure logging, etc.

        
        // Register handlers
        GlobalServiceLocator.Instance.Register(logger);

        HandlersDispatcher.AddHandler(new LoginHandler());
        HandlersDispatcher.AddHandler(new JsonHandler());
    }
}