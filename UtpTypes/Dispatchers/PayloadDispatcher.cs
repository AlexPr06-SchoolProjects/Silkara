using System.Reflection;
using UTP.Payload;

namespace UtpTypes.Dispatchers;

internal static class PayloadDispatcher
{
    private static readonly Dictionary<string, Type> PayloadTypes;

    static PayloadDispatcher()
    {
        PayloadTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IPayload).IsAssignableFrom(t)
                        && !t.IsInterface
                        && !t.IsAbstract)
            .ToDictionary(
                t => t.Name,
                t => t
            );
    }

    public static Type GetType(string payloadName)
        => PayloadTypes[payloadName];

    public static Type GetType(IPayload payload)
        => payload.GetType();

    public static bool TryGetType(string payloadName, out Type? type)
        => PayloadTypes.TryGetValue(payloadName, out type);
}
