using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using SikaraServer.Client.ClientIdentities.BasicClientIdentity;
using SilkaraServer.Client.Interfaces;

namespace SikaraServer.App;

internal class SikaraServerClass : BackgroundService, IDisposable
{
    private string serverIp = "127.0.0.1";
    private int serverPort = 123;
    private TcpListener _listener;
    private readonly ILogger<SikaraServerClass> _logger;
    private readonly List<Task> _clientTasks = new();

    private IClientManager _clientManager;
    public SikaraServerClass(ILogger<SikaraServerClass> logger, IClientManager clientManager) {
        _clientManager = clientManager;
        _listener = new TcpListener(IPAddress.Parse(serverIp), serverPort);
        _logger = logger;
    }
    public void Init()
    {
        _listener.Start();
        _logger.LogInformation("Server is listening...");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await RunServerAsync(stoppingToken);

    private async Task RunServerAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("SikaraServer is running...");
            Init();
            await ListenAsync(stoppingToken);
        }
        catch when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("SikaraServer is stopping due to cancellation request.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error: {ex.Message}");
        }
        finally
        {
            _listener.Stop();
            _logger.LogInformation("RunServerAsync method has stopped.");
        }
    }

    private async Task ListenAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync(stoppingToken);
                if (client.Client.RemoteEndPoint is IPEndPoint remoteEP)
                    LogNewConnectionAsync(remoteEP);

                IClientIdentity clientIdentity = ClientFactory.Instance.Create(client);
                _clientManager.AddClient(clientIdentity);
                Task task = Task.Run(() =>
                {
                    try{ clientIdentity.Processing(_logger);}
                    catch (Exception ex) { _logger.LogError($"ERROR: {ex.Message}"); }
                    finally
                    {
                        var id = clientIdentity.Id;
                        _clientManager.RemoveClient(clientIdentity.Id);
                        _logger.LogInformation("Client {ClientId} removed", id);
                    }
                }, stoppingToken);

                lock(_clientTasks) { _clientTasks.Add(task); }
                lock(_clientTasks) { _clientTasks.RemoveAll(t => t.IsCompleted); }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Loop stopped due token cancellation.");
                break;
            }
        }
    }

    private void LogNewConnectionAsync(IPEndPoint remoteEP)
    {
        string ip = remoteEP.Address.ToString();
        int port = remoteEP.Port;
        _logger.LogInformation($"{DateTime.Now.ToShortTimeString()} New client connected - {ip}:{port}");
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SikaraServer is shutting down gracefully...");
        _listener.Stop();

        Task[] tasksToWait;
        lock(_clientTasks) { tasksToWait = _clientTasks.ToArray(); }
        if (tasksToWait.Length > 0)
        {
            _logger.LogInformation("Waiting for {Count} clients to finish...", tasksToWait.Length);
        }
        await Task.WhenAny(Task.WhenAll(tasksToWait), Task.Delay(Timeout.Infinite, cancellationToken));
        await base.StopAsync(cancellationToken);
        _clientManager.Dispose();
        _logger.LogInformation("SikaraServer stopped cleanly(with  StopAsync method).");
    }
}
