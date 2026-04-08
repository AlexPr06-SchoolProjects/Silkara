using UtpTypes.UtpClientType;
using UtpTypes.Visitors;
using UtpTypes.Pipelines;
using UtpTypes.UtpMessageContext;
using UtpTypes.Services;
using UTP.UtpMessage.Interfaces;

namespace SilkaraClient.Client;

public class UtpClientAdapter
{
    private readonly UtpClient _utpClient;

    public UtpClientAdapter(UtpClient utpClient) => _utpClient = utpClient;

    public async Task SendMessageAsync(IUtpMessage utpMessage, CancellationToken ct = default) 
        => await _utpClient.SendMessageAsync(utpMessage, ct);
    
    public async Task<IUtpMessage> ReceiveMessageAsync(CancellationToken ct = default) 
        => await _utpClient.ReceiveMessageAsync(ct);
    
    public async Task HandleMessageAsync(
        IUtpMessage rawMessage,
        UtpPipeline pipeline,
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

    public async Task DisposeAsync() => await _utpClient.DisposeAsync();
}
