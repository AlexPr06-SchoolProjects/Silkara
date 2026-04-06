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

    public static bool TryGetType(string payloadName, out Type? type)
        => PayloadTypes.TryGetValue(payloadName, out type);
}
