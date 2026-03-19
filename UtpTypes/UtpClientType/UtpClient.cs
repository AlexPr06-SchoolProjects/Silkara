using UTP;
using UTP.Connection;
using UTP.Payload;
using UTP.UtpMessage;
using UtpTypes.Dispatchers;
using UTP.Constants;
using UTP.UtpMessage.Interfaces;

namespace UtpTypes.UtpClientType;

public class UtpClient : IAsyncDisposable
{
    private const string HEADER_PAYLOAD_TYPE_KEY = UtpConstants.Headers.PayloadTypeKey;
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
        Type payloadType = GetPayloadTypeFromHeaders(utpMessage.Headers);

        var method = typeof(UtpEngine)
            .GetMethod(nameof(UtpEngine.SendMessageAsync))!
            .MakeGenericMethod(payloadType!);

        await (Task)method.Invoke(_engine, new object[] { utpMessage, ct })!;
    }

    public async Task<IUtpMessage> ReceiveMessageAsync(CancellationToken ct = default)
    {
        var (actionCode, headers, payloadLen) = await _engine.ReceiveBeforePayloadAsync();

        Type payloadType = GetPayloadTypeFromHeaders(headers);

        var messageType = typeof(UtpMessage<>).MakeGenericType(payloadType!);

        var message = Activator.CreateInstance(messageType, new object[] { actionCode, headers });

        var result =  await UtpMessageCacheManager.Execute(
                payloadType!,
                _engine,
                payloadLen,
                message!,
                ct
            );

        return (IUtpMessage)result;
    }

    private Type GetPayloadTypeFromHeaders(IDictionary<string, string> headers)
    {
        string payloadName = headers[HEADER_PAYLOAD_TYPE_KEY];

        if (!PayloadDispatcher.TryGetType(payloadName, out var payloadType))
            throw new Exception($"Unknown payload: {payloadName}");

        if (payloadType == null)
            throw new Exception("Payload type not found");

        if (!typeof(IPayload).IsAssignableFrom(payloadType))
            throw new Exception($"Invalid payload type: {payloadType}");

        return payloadType!;
    }

    public async ValueTask DisposeAsync()
    {
        await _engine.DisposeAsync();
    }
}

internal static class UtpMessageCacheManager
{
    private static readonly Dictionary<
           Type,
           Func<UtpEngine, int, object, CancellationToken, Task<object>>
       > _cache = new();

    public static async Task<object> Execute(
        Type payloadType,
        UtpEngine utpEngine,
        int payloadLen,
        object message,
        CancellationToken ct
        )
    {
        if (!_cache.TryGetValue(payloadType, out var handler))
        {
            handler = CreateDelegate(payloadType);
            _cache.Add(payloadType, handler);
        }

        return await handler(utpEngine, payloadLen, message, ct);
    }

    private static Func<UtpEngine, int, object, CancellationToken, Task<object>> CreateDelegate(Type paylaodType)
    {
        var method = typeof(UtpEngine)
            .GetMethod(nameof(UtpEngine.ReceivePayloadAsync))!
            .MakeGenericMethod(paylaodType);

        return async (engine, payloadLen, message, ct) =>
        {
            var task = (Task)method.Invoke(engine, new object[]
            {
                payloadLen,
                message,
                ct
            })!;

            await task;

            return task
                .GetType()
                .GetProperty("Result")!
                .GetValue(task)!;
        };
    }
}
