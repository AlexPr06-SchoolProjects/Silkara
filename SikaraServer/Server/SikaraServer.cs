using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using SilkaraServer.Client.Managers.Client;
using SilkaraServer.Client.Factories;
using SilkaraServer.Client.Identities;
using SilkaraServer.Settings;

namespace SilkaraServer.Server;

internal class SikaraServerClass : BackgroundService
{
    private readonly GlobalServerSetuper _setuper;
    private readonly string _serverIp = GlobalServerSettings.IpAddress;
    private readonly int _serverPort = GlobalServerSettings.Port;
    private readonly TcpListener _listener;
    private readonly ILogger<SikaraServerClass> _logger;
    private readonly List<Task> _clientTasks = new();
    private readonly IClientManager _clientManager;
    public SikaraServerClass(GlobalServerSetuper setuper, ILogger<SikaraServerClass> logger, IClientManager clientManager) {
        _setuper = setuper;
        _clientManager = clientManager;
        _listener = new TcpListener(IPAddress.Parse(_serverIp), _serverPort);
        _logger = logger;
    }

    public void Init()
    {
        GlobalServerSetup();
        _listener.Start();
        _logger.LogInformation("Server is listening on {Ip}:{Port}...", _serverIp, _serverPort);
    }

    private void GlobalServerSetup()
    {
        _setuper.Setup(logger: _logger);
    }

    protected override async Task ExecuteAsync(CancellationToken ct) => await RunServerAsync(ct);

    private async Task RunServerAsync(CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("SikaraServer is running...");
            Init();
            await ListenAsync(ct);
        }
        catch when (ct.IsCancellationRequested)
        {
            _logger.LogInformation("SikaraServer is stopping due to cancellation request.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Unexpected warning: {ex.Message}");
        }
        finally
        {
            _listener.Stop();
            _logger.LogInformation("Server stopped.");
        }
    }

    private async Task ListenAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync(ct);
                if (client.Client.RemoteEndPoint is IPEndPoint remoteEp)
                    LogNewConnectionAsync(remoteEp);

                IClientIdentity clientIdentity = ClientFactory.Instance.Create(client);
                _clientManager.AddClient(clientIdentity);
                var task = HandleClientAsync(clientIdentity, ct);

                lock (_clientTasks) { _clientTasks.Add(task); }
                _ = task.ContinueWith(t => { lock (_clientTasks) { _clientTasks.Remove(t); } }, ct);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Loop stopped due token cancellation.");
        }
        finally{ _logger.LogInformation("ListenAsync loop iteration finished."); }
    }

    private async Task HandleClientAsync(IClientIdentity client, CancellationToken ct)
    {
        try { await client.Processing(_logger, ct); }
        catch (Exception ex) { _logger.LogError($"ERROR: {ex.Message}"); }
        finally
        {
            var id = client.IdManager.ClientId;
            await _clientManager.RemoveClient(id);
            _logger.LogInformation("Client {ClientId} removed", id);
        }
    }

    private void LogNewConnectionAsync(IPEndPoint remoteEp)
    {
        string ip = remoteEp.Address.ToString();
        int port = remoteEp.Port;
        _logger.LogInformation($"{DateTime.Now.ToShortTimeString()} New client connected - {ip}:{port}");
    }

    private Task[] GetTasksToWait()
    {
        lock(_clientTasks) { return _clientTasks.ToArray(); }
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        _logger.LogInformation("SikaraServer is shutting down gracefully...");
        _listener.Stop();

        Task[] tasksToWait = GetTasksToWait();
        if (tasksToWait.Length > 0)
        {
            _logger.LogInformation("Waiting for {Count} clients to finish...", tasksToWait.Length);
            await Task.WhenAny(Task.WhenAll(tasksToWait), Task.Delay(Timeout.Infinite, ct));
        }
        await base.StopAsync(ct);
        await _clientManager.DisposeAsync();
        _logger.LogInformation("SikaraServer stopped cleanly(with  StopAsync method).");
    }
}
