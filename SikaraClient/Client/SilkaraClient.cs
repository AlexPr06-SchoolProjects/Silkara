using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using UTP.Connection;
using UTP.UtpMessage;
using UtpTypes.Actions;
using UtpTypes.PayloadTypes;
using UtpTypes.UtpClientType;
using UTP.UtpMessage.Interfaces;
using UtpTypes.Pipelines;
using UtpTypes.Routers;
using UtpTypes.Services;
using SilkaraClient.Settings;

namespace SilkaraClient.Client;



internal class SilkaraClientClass(ILogger<SilkaraClientClass> logger) : BackgroundService
{
    private readonly TcpClient _tcpClient = new TcpClient();
    private UtpClientAdapter? _utpClient;
    private UtpPipeline? _pipeline;
    private UtpRouter? _router;
    private StateServiceLocator? _stateServiceLocator;
    private readonly string _serverIp = GlobalClientSettings.IpAddress;
    private readonly int _serverPort = GlobalClientSettings.Port;
    protected override async Task ExecuteAsync(CancellationToken ct)
        => await RunClientAsync(ct);
    
    private void GlogalClientSetup() => GlobalClientSetuper.Instance.Setup(logger: logger);

    private async Task RunClientAsync(CancellationToken ct)
    {
        GlogalClientSetup();

        if(await ConnectedSuccessfully(ct) is false) return;

        if (_utpClient == null)                    {
            logger.LogWarning("UtpClient is not initialized.");
            return;
        }

        ProcessSetup();

        
        // TEMPORARY
        var message = CreateDefaultMessage();
        // TEMPORARY


        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _utpClient.SendMessageAsync(message, ct);
                    logger.LogInformation("Message was sent");
                    IUtpMessage received = await _utpClient.ReceiveMessageAsync(ct);
                    logger.LogInformation("Message was received");
                    logger.LogInformation($"Received message: {received.ActionCode}, {received.Headers}");
                    await _utpClient.HandleMessageAsync(received, _pipeline!, _stateServiceLocator, ct);
                }
                catch (OperationCanceledException)
                {
                    logger.LogInformation("Connection attempt was canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError($"{ex.Message}");
                    break;
                }
                Console.ReadLine();
            }
        }
        finally
        {
            await DisconnectAsync();
        }
    }

    private async Task ConnectToServerAsync(CancellationToken ct)
    {
        try
        {
             await _tcpClient.ConnectAsync(_serverIp, _serverPort, ct);
            UtpConnection utpConnection = new UtpConnection(_tcpClient.Client, ct);
            _utpClient = new UtpClientAdapter(new UtpClient(utpConnection));
            logger.LogInformation($"Connected to server at {_serverIp}:{_serverPort}");
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Connection attempt was canceled.");
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(@$"Failed to connect to server at {_serverIp}:{_serverPort}\n
                Exception message: {ex.Message}");
            throw;
        }
    }

    private async Task<bool> ConnectedSuccessfully(CancellationToken ct)
    {
        try
        {
            await ConnectToServerAsync(ct);
            return true;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Connection attempt was canceled.");
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError($"ERROR: {ex.Message}");
            return false;
        }
    }


    private void ProcessSetup()
    {
        // Instantiate services
        _stateServiceLocator = new StateServiceLocator();

        _router = new UtpRouter();
        _pipeline = new UtpPipeline(_router);

        // Register middleware

        // Register services in the service locator for middleware and routers
        if (_utpClient is not null) 
            _stateServiceLocator.Register(_utpClient);
    }

    private UtpMessage<JsonPayload> CreateDefaultMessage()
    {     
        string bigData = new string('A', 1024 * 1024 * 1);
        var bigPayload = new JsonPayload(
            1,
            bigData
        );

        var message = new UtpMessage<JsonPayload>(
            actionCode: (short)MessageCode.Json,
            headers: new Dictionary<string, string>
            {
                ["Test"] = "BigDataCheck",
                ["Mode"] = "Stress",
                ["Encoding"] = "utf-8",
                ["TraceId"] = Guid.NewGuid().ToString(),
                ["User-Agent"] = "UTP-StressClient/1.0",
                ["X-Random-1"] = new string('Z', 200),
                ["X-Random-2"] = new string('Y', 300),
            },
            payload: bigPayload
        );

        return message;
    }

    private async Task DisconnectAsync()
    {
        if (_utpClient != null)
        {
            await _utpClient.DisposeAsync();
            logger.LogInformation("Disconnected from server.");
        }
        try
        {
            if (_tcpClient.Client is { Connected: true })
                _tcpClient.Client.Shutdown(SocketShutdown.Send);
        }
        catch (ObjectDisposedException) { /* Already disposed */ }
        catch (Exception) { /* Socket might already be in a bad state */ }
        finally
        {
            _tcpClient.Close();
            logger.LogInformation("TCP connection closed.");
        }
    }
}