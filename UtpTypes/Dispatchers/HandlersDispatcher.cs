using System.Reflection;
using UtpTypes.Handlers;
using UtpTypes.Services;

namespace UtpTypes.Dispatchers;

public class HandlersDispatcher
{
    private static readonly Lazy<Dictionary<short, UtpHandlerBase>> Handlers =
        new(() => Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(UtpHandlerBase).IsAssignableFrom(t) 
                        && t is {IsInterface: false, IsAbstract: false})
            .Select(t => (UtpHandlerBase)Activator.CreateInstance(t, GlobalServiceLocator.Instance)!)
            .ToDictionary(h => h.ActionCode, h => h));
    public static HandlersDispatcher Instance { get; } = new HandlersDispatcher();

    private HandlersDispatcher() { }

    public  UtpHandlerBase? TryGetHandler(short actionCode)
        => Handlers.Value.GetValueOrDefault(actionCode);
}