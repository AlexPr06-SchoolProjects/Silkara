using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using SilkaraServer.Presentation.Server;
using UTP.Connection;
using UTP.Exceptions;
using UTP.UtpMessage.Interfaces;
using SilkaraServer.Infrastructure.Middleware;
using UtpTypes.Pipelines;
using UtpTypes.Routers;
using UtpTypes.Services;
using StackExchange.Redis;
using SilkaraServer.Infrastructure;
using SilkaraServer.Domain.Id;
using SilkaraServer.Domain.Services.StateServicesConcretes;

namespace SilkaraServer.Domain.Identities;

internal class ClientIdentity(TcpClient tcpClient, ClientIdManager clientIdManager, IDatabase redisDb)
    : IClientIdentity
{
    private UtpPipeline? _pipeline;
    private UtpRouter? _router;
    private StateServiceLocator? _stateServiceLocator;
    private ClientStateService? _clientStateService;
    public IClientIdManager IdManager { get; } = clientIdManager;
    private UtpServerClient? _utpClient;
    private bool _disposed;

    public async Task Processing(ILogger<SilkaraServerClass> logger, CancellationToken ct)
    {
        InstantiateConnection(logger, ct);
        if (_utpClient == null)
        {
            logger.LogWarning($"Failed to instantiate UtpClient for Client with ID: {IdManager.ClientId}");
            return;
        }

        ProcessSetup();

        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    IUtpMessage received = await _utpClient.ReceiveMessageAsync(ct);
                    await _utpClient.HandleMessageAsync(received, _pipeline!, clientId: IdManager.ClientId, _stateServiceLocator, ct);
                }
                catch (ConnectionClosedPrematurelyException ex)
                {
                    logger.LogInformation($"ConnectionClosedPrematurelyException: {ex.Message}. Client with ID: {IdManager.ClientId}");
                    break;
                }
                catch (RemotePeerDisconnectedException ex)
                {
                    logger.LogInformation($"RemotePeerDisconnectedException: {ex.Message}. Client with ID: {IdManager.ClientId}");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Unexpected exception: {ex.Message}. Client with ID: {IdManager.ClientId}");
                    break;
                }
            }
        }
        finally { CloseConnection(logger); }
    }

    private void ProcessSetup()
    {
        // Instantiate services
        _stateServiceLocator = new StateServiceLocator();
        _clientStateService = new ClientStateService();

        _router = new UtpRouter();
        _pipeline = new UtpPipeline(_router);

        // Register middleware
        _pipeline.Use(new LoggingMiddleware(_stateServiceLocator));
        _pipeline.Use(new RateLimitMiddleware(_stateServiceLocator));
        _pipeline.Use(new StateValidationMiddleware(_stateServiceLocator));

        // Register services in the service locator for middleware and routers
        if (_utpClient is not null)
            _stateServiceLocator.Register(_utpClient);
        _stateServiceLocator.Register(_clientStateService);
        _stateServiceLocator.Register(IdManager);
        _stateServiceLocator.Register(redisDb);
    }

    private void InstantiateConnection(ILogger<SilkaraServerClass> logger, CancellationToken ct)
    {
        _utpClient = new UtpServerClient(new UtpConnection(tcpClient.Client, ct));
        logger.LogInformation("Connection with the Client ({ClientId} was instatntiated.", IdManager.ClientId);
    }

    private void CloseConnection(ILogger<SilkaraServerClass> logger)
    {
        try
        {
            if (tcpClient.Client.Connected)
            {
                tcpClient.Client.Shutdown(SocketShutdown.Both);
            }
        }
        catch (SocketException ex)
        {
            logger.LogWarning($"SocketException while shutting down connection for Client {IdManager.ClientId}: {ex.Message}");
        }
        catch (ObjectDisposedException ex)
        {
            logger.LogWarning($"ObjectDisposedException while shutting down connection for Client {IdManager.ClientId}: {ex.Message}");
        }
        finally
        {
            tcpClient.Close();
            logger.LogInformation("Connection with client {ClientId} closed.", IdManager.ClientId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            if (_utpClient != null)
                await _utpClient.DisposeAsync();
            tcpClient.Close();
            tcpClient.Dispose();
        }
        _disposed = true;
    }

    public Task Processing(ILogger<SilkaraServerIdenity> logger, CancellationToken token)
    {
        //TODO: Implement processing method
        throw new NotImplementedException();
    }
}
