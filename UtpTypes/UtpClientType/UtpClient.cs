using UTP;
using UTP.Connection;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage;
using UTP.UtpMessage.Interfaces;
using UtpTypes.Dispatchers;
using UtpTypes.PayloadTypes;
namespace UtpTypes.UtpClientType;


public class UtpClient : IAsyncDisposable
{
    private const string HeaderPayloadTypeKey = UtpConstants.Headers.PayloadTypeKey;
    private readonly UtpEngine _engine;

    public UtpClient(UtpConnection connection)
    {
        _engine = new UtpEngine(connection);
    }

    public async Task SendMessageAsync<TPayload>(UtpMessage<TPayload> utpMessage, CancellationToken ct = default)
        where TPayload : IPayload
            => await _engine.SendMessageAsync(utpMessage, ct);
   
    public async Task SendMessageAsync (IUtpMessage utpMessage, CancellationToken ct = default)
    {
        Type? payloadType = GetPayloadTypeFromHeaders(utpMessage.Headers);

        var method = typeof(UtpEngine)
            .GetMethod(nameof(UtpEngine.SendMessageAsync))!
            .MakeGenericMethod(payloadType ?? typeof(EmptyPayload));

        await (Task)method.Invoke(_engine, [utpMessage, ct])!;
    }

    public async Task<IUtpMessage> ReceiveMessageAsync(CancellationToken ct = default)
    {
        var (actionCode, headers, payloadLen) = await _engine.ReceiveBeforePayloadAsync(ct);
        Type? payloadType = GetPayloadTypeFromHeaders(headers);
        var messageType = typeof(UtpMessage<>).MakeGenericType(payloadType ?? typeof(EmptyPayload));
        var message = Activator.CreateInstance(messageType, [actionCode, headers]);

        object result = message!;

        if (payloadLen > 0) 
        {
            if (payloadType == null)
                throw new Exception("Payload exists but type is missing");

            result = await UtpMessageCacheManager.Execute(
                payloadType,
                _engine,
                payloadLen,
                message!,
                ct
            );
        }

        return (IUtpMessage)result;
    }

    private Type? GetPayloadTypeFromHeaders(IDictionary<string, string> headers)
    {
        if (!headers.TryGetValue(HeaderPayloadTypeKey, out var payloadName))
            return null;

        if (!PayloadsDispatcher.TryGetType(payloadName, out var payloadType))
            throw new Exception($"Unknown payload: {payloadName}");

        if (!typeof(IPayload).IsAssignableFrom(payloadType))
            throw new Exception($"Invalid payload type: {payloadType}");

        return payloadType;
    }

    public async ValueTask DisposeAsync()
    {
        await _engine.DisposeAsync();
    }
}

internal static class UtpMessageCacheManager
{
    // PayloadType -> Func which creates UtpMessage<PayloadType>
    private static readonly Dictionary<
           Type, 
           Func<UtpEngine, int, object, CancellationToken, Task<object>>> Cache = new();

    public static async Task<object> Execute(
        Type payloadType,
        UtpEngine utpEngine,
        int payloadLen,
        object message,
        CancellationToken ct
        )
    {
        if (!Cache.TryGetValue(payloadType, out var handler))
        {
            handler = CreateDelegate(payloadType);
            Cache.Add(payloadType, handler);
        }

        return await handler(utpEngine, payloadLen, message, ct);
    }

    private static Func<UtpEngine, int, object, CancellationToken, Task<object>> CreateDelegate(Type payloadType)
    {
        var method = typeof(UtpEngine)
            .GetMethod(nameof(UtpEngine.ReceivePayloadAsync))!
            .MakeGenericMethod(payloadType); // Key moment!

        return async (engine, payloadLen, message, ct) =>
        {
            var task = (Task)method.Invoke(engine,
            [
                payloadLen,
                message,
                ct
            ])!;

            await task;

            return task
                .GetType()
                .GetProperty("Result")!
                .GetValue(task)!;
        };
    }
}
