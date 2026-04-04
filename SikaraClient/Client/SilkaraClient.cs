using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using UTP.Connection;
using UTP.UtpMessage;
using UtpTypes.Actions;
using UtpTypes.PayloadTypes;
using UtpTypes.UtpClientType;

namespace SilkaraClient.Client;

internal class SilkaraClientClass(ILogger<SilkaraClientClass> logger) : BackgroundService
{
    private readonly TcpClient _tcpClient = new TcpClient();
    private UtpClient _utpClient = null!;
    private readonly string _serverIp = "127.0.0.1";
    private readonly int _serverPort = 123;
    protected override async Task ExecuteAsync(CancellationToken ct)
        => await RunClientAsync(ct);

    private async Task RunClientAsync(CancellationToken ct)
    {
        string bigData = new string('A', 1024 * 1024 * 128);
                var bigPayload = new JsonPayload(
                    1,
                    bigData
                );

                var message = new UtpMessage<JsonPayload>(
                    actionCode: (short)ActionCode.Ping,
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

        try
        {
            await ConnectToServerAsync(ct);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Connection attempt was canceled.");
            return;
        }
        catch (Exception ex)
        {
            logger.LogError($"ERROR: {ex.Message}");
            return;
        }

        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    await _utpClient.SendMessageAsync(message, ct);
                    logger.LogInformation("Message was sent");
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning("Connection attempt was canceled.");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"{ex.Message}");
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
            _utpClient = new UtpClient(utpConnection);
            logger.LogInformation($"Connected to server at {_serverIp}:{_serverPort}");
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Connection attempt was canceled.");
            throw;
        }
        catch (Exception)
        {
            logger.LogWarning("Failed to connect to server at {ServerIp}:{ServerPort}", _serverIp, _serverPort);
            throw;
        }
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
        catch (ObjectDisposedException) { /* Уже закрыто, игнорируем */ }
        catch (Exception) { /* Сокет мог быть уже в плохом состоянии */ }
        finally
        {
            _tcpClient.Close();
            logger.LogInformation("TCP connection closed.");
        }
    }

    

    //private async Task RequestChatCreation()
    //{
    //    var packet = new SikaraPacket<ChatCreationEnum>(ChatCreationEnum.CreateChat, "New-chat");
    //    await JsonManager.SendMessageAsync<SikaraPacket<ChatCreationEnum>, ChatCreationEnum>(packet, writer);
    //}
}
