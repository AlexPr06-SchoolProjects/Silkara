using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using UTP.Connection;
using UTP.UtpMessage;
using UtpTypes;
using UtpTypes.Actions;
using UtpTypes.UtpClientType;

namespace SikaraClient.Client;
internal class SikaraClientClass : BackgroundService
{
	private ILogger<SikaraClientClass> _logger;

    private TcpClient _tcpClient;
    private UtpClient _utpClient;

    private string serverIp = "127.0.0.1";
    private int serverPort = 123;

    public SikaraClientClass(ILogger<SikaraClientClass> logger)
    {
        _tcpClient = new TcpClient();
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        => await RunCLientAsync(stoppingToken);

    private async Task RunCLientAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ConnectToServerAsync();
            string bigData = new string('A', 1023 * 7);
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

            await _utpClient.SendMessageAsync(message);
            _logger.LogInformation("Message was sent");
            Console.ReadLine();
        }
        catch (Exception ex)
        {
            _logger.LogError($"ERROR: {ex.Message}");
        }
    }

    private async Task ConnectToServerAsync()
    {
        await _tcpClient.ConnectAsync(serverIp, serverPort);
        UtpConnection utpConnection = new UtpConnection(_tcpClient.Client);
        _utpClient = new UtpClient(utpConnection);
        _logger.LogInformation($"Connected to server at {serverIp}:{serverPort}");
    }



    //private async Task RequestChatCreation()
    //{
    //    var packet = new SikaraPacket<ChatCreationEnum>(ChatCreationEnum.CreateChat, "New-chat");
    //    await JsonManager.SendMessageAsync<SikaraPacket<ChatCreationEnum>, ChatCreationEnum>(packet, writer);
    //}
}
