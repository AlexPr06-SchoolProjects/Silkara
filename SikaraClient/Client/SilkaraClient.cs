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
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await RunClientAsync(stoppingToken);

    private async Task RunClientAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ConnectToServerAsync();
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

            await _utpClient.SendMessageAsync(message, stoppingToken);
            logger.LogInformation("Message was sent");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            logger.LogError($"ERROR: {ex.Message}");
        }
    }

    private async Task ConnectToServerAsync()
    {
        await _tcpClient.ConnectAsync(_serverIp, _serverPort);
        UtpConnection utpConnection = new UtpConnection(_tcpClient.Client);
        _utpClient = new UtpClient(utpConnection);
        logger.LogInformation($"Connected to server at {_serverIp}:{_serverPort}");
    }

    //private async Task RequestChatCreation()
    //{
    //    var packet = new SikaraPacket<ChatCreationEnum>(ChatCreationEnum.CreateChat, "New-chat");
    //    await JsonManager.SendMessageAsync<SikaraPacket<ChatCreationEnum>, ChatCreationEnum>(packet, writer);
    //}
}
