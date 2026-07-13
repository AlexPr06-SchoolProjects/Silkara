using System.Reflection;
using UtpTypes.Handlers;

namespace UtpTypes.Dispatchers;

public class HandlersDispatcher
{
    private static readonly Lazy<Dictionary<short, UtpHandlerBase>> Handlers =
        new(() => Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => typeof(UtpHandlerBase).IsAssignableFrom(t) 
                        && t is {IsInterface: false, IsAbstract: false})
            .Select(t => (UtpHandlerBase)Activator.CreateInstance(t, [])!)
            .ToDictionary(h => h.ActionCode, h => h));
    public static HandlersDispatcher Instance { get; } = new HandlersDispatcher();

    public static void AddHandler(UtpHandlerBase handler)
    {
        if (Handlers.Value.ContainsKey(handler.ActionCode))
            throw new InvalidOperationException($"Handler for action code {handler.ActionCode} already exists.");

        Handlers.Value[handler.ActionCode] = handler;
    }

    public static void RemoveHandler(short actionCode) => Handlers.Value.Remove(actionCode);

    private HandlersDispatcher() { }

    public  UtpHandlerBase? TryGetHandler(short actionCode)
        => Handlers.Value.GetValueOrDefault(actionCode);
}