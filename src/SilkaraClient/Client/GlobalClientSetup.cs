using Microsoft.Extensions.Logging;
using SilkaraClient.Handlers;
using UtpTypes.Dispatchers;
using UtpTypes.Services;

namespace SilkaraClient.Client;

internal class GlobalClientSetuper
{
    public static GlobalClientSetuper Instance { get; } = new GlobalClientSetuper();

    private GlobalClientSetuper() { }
    public void Setup(ILogger logger)
    {
        // Perform any global setup for the server here
        // For example, you can register global services, configure logging, etc.

        GlobalServiceLocator.Instance.Register(logger);

        // Register handlers
        HandlersDispatcher.AddHandler(new ClientUnauthorizedHandler());
    }
}
