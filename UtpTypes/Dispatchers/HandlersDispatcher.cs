using System.Reflection;
using UtpTypes.Handlers;
using IServiceProvider = System.IServiceProvider;

namespace UtpTypes.Dispatchers;

public class HandlersDispatcher(IServiceProvider serviceProvider)
{
    private static readonly Dictionary<short, IUtpHandler> Handlers;

    static HandlersDispatcher()
    {
        Handlers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(IUtpHandler).IsAssignableFrom(t) 
                        && t is {IsInterface: false, IsAbstract: false})
            .Select(t => (IUtpHandler)Activator.CreateInstance(t, true)!)
            .ToDictionary(h => h.ActionCode, h => h);
    }

    public static bool TryGetHandler(short actionCode, out IUtpHandler? handler)
        => Handlers.TryGetValue(actionCode, out handler);
}