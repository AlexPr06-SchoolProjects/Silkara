using UTP;
using UTP.Connection;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage;
using UTP.UtpMessage.Interfaces;
using UtpTypes.Dispatchers;
using UtpTypes.PayloadTypes;
using UtpTypes.Visitors;
using UtpTypes.Pipelines;
using UtpTypes.UtpMessageContext;
using UtpTypes.Services;

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

    /* TODO: 

    UtpClient.ReceiveMessageAsync() - does it job perfectly in terms of deserialization and converting to the actual Payload type. 

    BUT it requires up to 5 - 8 times more memory than actual payload size!!! - that's terrible!
    I assume that the problem stems from the approach I chose to convert IPayload to actual Payload type - reflection.
    SHOULD BE REWRITEN WITH MORE EFFICIENT REFLECTION APPROACH. - but it is for future.

     TODO: */
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

    public async Task HandleMessageAsync(
        IUtpMessage rawMessage, 
        UtpPipeline pipeline, 
        Guid clientId, 
        CancellationToken ct = default
        )
    {
        var visitor = new ContextCreatorVisitor(clientId, ct);
        UtpContext context = rawMessage.Accept(visitor);
        await pipeline.Build().Invoke(context);
        rawMessage.Dispose();
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
