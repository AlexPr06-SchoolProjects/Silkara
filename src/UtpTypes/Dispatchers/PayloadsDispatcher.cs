using System.Reflection;
using UTP.Payload;

namespace UtpTypes.Dispatchers;

internal static class PayloadsDispatcher
{
    private static readonly Dictionary<string, Type> PayloadTypes;

    static PayloadsDispatcher()
    {
        PayloadTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IPayload).IsAssignableFrom(t)
                        && t is { IsInterface: false, IsAbstract: false })
            .ToDictionary(
                t => t.Name,
                t => t
            );
    }

    public static void AddType(Type payloadType) 
        => PayloadTypes[payloadType.Name] = payloadType;

    public static void AddPayload(IPayload payload) 
        => AddType(payload.GetType());

    public static void RemoveType(string payloadName) 
        => PayloadTypes.Remove(payloadName);

    public static bool TryGetType(string payloadName, out Type? type)
        => PayloadTypes.TryGetValue(payloadName, out type);
}
