using JsonManagerLib.Classes;
using JsonManagerLib.Enums;
using JsonManagerLib.Records;
using Microsoft.Extensions.Logging;
using SikaraServer.App;
using SilkaraServer.Client.Interfaces;
using System.Net.Sockets;
using UTP;

namespace SikaraServer.Client.ClientIdentities.BasicClientIdentity;

internal class ClientIdentity : IClientIdentity
{
    public Guid Id { get; }
    private readonly TcpClient _tcpClient;
    private NetworkStream _netStream = null!;
    private bool _disposed;

    public ClientIdentity(TcpClient tcpClient, Guid id)
    {
        _tcpClient = tcpClient;
        Id = id;
    }

    public bool IsActive => throw new NotImplementedException();

    public void Processing(ILogger<SikaraServerClass> _logger)
    {
        try
        {
            _netStream = _tcpClient.GetStream();

            UtpEngine engine = new UtpEngine(_netStream);

            // ------------------------- My olf logic -------------------------

            // 1. Попытка подключится к конркетному чату ИЛИ создать новый
            // 3. Подключение к чату
            // 4.Розрыв связи с SikaraServer
            using StreamReader reader = new StreamReader(_netStream);
            using StreamWriter writer = new StreamWriter(_netStream) { AutoFlush = true };

            var data = JsonManager.ReadMessage<SikaraPacket<ChatCreationEnum>, ChatCreationEnum>(reader);
            _logger.LogInformation("Received data from client({ClientId}): {ClientData}", Id, data);

            // -------------------------- My olf logic -------------------------

            //while (true)
            //{
            //    UtpMessage<JsonPayload> message = engine.Receive<JsonPayload>();

            //    // ...
            //}

            //// ...

        }
        catch (Exception ex)
        {
            _logger.LogError($"Error handling client: {ex.Message}");
        }
        finally
        {
            _tcpClient.Close();
            _logger.LogInformation("Connection with client {ClientId} closed.", Id);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _netStream?.Dispose();
            _tcpClient?.Close();
            _tcpClient?.Dispose();
        }

        _disposed = true;
    }
}
