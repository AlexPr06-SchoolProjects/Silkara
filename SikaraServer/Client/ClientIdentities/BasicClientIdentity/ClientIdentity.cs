using JsonManagerLib.Classes;
using JsonManagerLib.Enums;
using JsonManagerLib.Records;
using Microsoft.Extensions.Logging;
using SilkaraServer.Server;
using SilkaraServer.Client.Interfaces;
using System.Net.Sockets;
using UTP.Connection;
using UTP.UtpMessage.Interfaces;
using UtpTypes.UtpClientType;
using UTP.Exceptions;
using UtpTypes.Middleware.MiddlewareConcretes;
using UtpTypes.Pipelines;
using UtpTypes.Routers;
using UtpTypes.Services;

namespace SilkaraServer.Client.ClientIdentities.BasicClientIdentity;

internal class ClientIdentity(TcpClient tcpClient, Guid id) : IClientIdentity
{
    public Guid Id { get; } = id;
    private UtpClient? _utpClient;
    private bool _disposed;

    public async Task Processing(ILogger<SikaraServerClass> logger, CancellationToken ct)
    {
        InstantiateConnection(logger, ct);
        if (_utpClient == null)
        {
            logger.LogWarning($"Failed to instantiate UtpClient for Client with ID: {Id}");
            return;
        }
        // Register services in the service locator for middleware and routers

        UtpRouter router = new UtpRouter();
        UtpPipeline pipeline = new UtpPipeline(router);
        pipeline.Use(new LoggingMiddleware(GlobalServiceLocator.Instance));

        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    IUtpMessage received = await _utpClient.ReceiveMessageAsync(ct);
                    await _utpClient.HandleMessageAsync(received, pipeline, clientId: Id, ct);
                }
                catch (ConnectionClosedPrematurelyException ex) 
                {
                    logger.LogInformation($"ConnectionClosedPrematurelyException: {ex.Message}. Client with ID: {Id}");
                    break;
                }
                catch (RemotePeerDisconnectedException ex)
                {
                    logger.LogInformation($"RemotePeerDisconnectedException: {ex.Message}. Client with ID: {Id}");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Unexpected exception: {ex.Message}. Client with ID: {Id}");
                    break;
                }
            }
        }
        finally { CloseConnection(logger); }
    }

    private void InstantiateConnection(ILogger<SikaraServerClass> logger, CancellationToken ct)
    {        
        _utpClient = new UtpClient(new UtpConnection(tcpClient.Client, ct));
        logger.LogInformation("Connection with the Client ({ClientId} was instatntiated.", Id);
    }

    private void CloseConnection(ILogger<SikaraServerClass> logger)
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
            logger.LogWarning($"SocketException while shutting down connection for Client {Id}: {ex.Message}");
        }
        catch (ObjectDisposedException ex)
        {
            logger.LogWarning($"ObjectDisposedException while shutting down connection for Client {Id}: {ex.Message}");
        }
        finally 
        {
            tcpClient.Close();
            logger.LogInformation("Connection with client {ClientId} closed.", Id);
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
}
