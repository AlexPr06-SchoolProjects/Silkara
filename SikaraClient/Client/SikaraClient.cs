using JsonManagerLib.Classes;
using JsonManagerLib.Enums;
using JsonManagerLib.Records;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace SikaraClient.Client;

internal class SikaraClientClass : BackgroundService
{
	private ILogger<SikaraClientClass> _logger;
    private string serverIp = "127.0.0.1";
    private int serverPort = 123;

    public SikaraClientClass(ILogger<SikaraClientClass> logger)
    {
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
		try
		{
            await ConnectToServerAsync();
        }
		catch (Exception ex)
		{
            _logger.LogError($"ERROR: {ex.Message}");
        }
    }

    private async Task ConnectToServerAsync()
    {
        TcpClient client = new TcpClient();
        await client.ConnectAsync(serverIp, serverPort);
        _logger.LogInformation($"Connected to server at {serverIp}:{serverPort}");

        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream);
        using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        await RequestChatCreation(stream, reader, writer);
    }

    private async Task RequestChatCreation(
        NetworkStream networkStream,
        StreamReader reader,
        StreamWriter writer)
    {
        var packet = new SikaraPacket<ChatCreationEnum>(ChatCreationEnum.CreateChat, "New-chat");
        await JsonManager.SendMessageAsync<SikaraPacket<ChatCreationEnum>, ChatCreationEnum>(packet, writer);
    }
}
