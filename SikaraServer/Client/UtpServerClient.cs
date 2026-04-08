using UtpTypes.UtpClientType;
using UTP.Connection;
using UtpTypes.Visitors;
using UtpTypes.Pipelines;
using UtpTypes.UtpMessageContext;
using UtpTypes.Services;
using UTP.UtpMessage.Interfaces;

namespace SilkaraServer.Client;
public class UtpServerClient(UtpConnection connection) : UtpClient(connection)
{
    public async Task HandleMessageAsync(
        IUtpMessage rawMessage,
        UtpPipeline pipeline,
        Guid clientId,
        IServiceLocator? serviceLocator = null,
        CancellationToken ct = default
    )
    {
        var visitor = new ContextCreatorVisitor(ct);
        UtpContext context = rawMessage.Accept(visitor);
        if (serviceLocator != null)
            context.ServiceLocator = serviceLocator;
        await pipeline.Build().Invoke(context);
        rawMessage.Dispose();
    }
}
