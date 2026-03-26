using JsonManagerLib.Classes;
using JsonManagerLib.Enums;
using JsonManagerLib.Records;
using Microsoft.Extensions.Logging;
using SikaraServer.App;
using SilkaraServer.Client.Interfaces;
using System.Net.Sockets;
using System.Text;
using UTP.Connection;
using UTP.UtpMessage;
using UTP.UtpMessage.Interfaces;
using UtpTypes.Actions;
using UtpTypes.PayloadTypes;
using UtpTypes.UtpClientType;

namespace SikaraServer.Client.ClientIdentities.BasicClientIdentity;

internal class ClientIdentity : IClientIdentity, IAsyncDisposable
{
    public Guid Id { get; }
    private readonly TcpClient _tcpClient;
    private UtpClient _utpClient = null!;
    private bool _disposed;

    public ClientIdentity(TcpClient tcpClient, Guid id)
    {
        _tcpClient = tcpClient;
        Id = id;
    }

    public bool IsActive => throw new NotImplementedException();

    public async Task Processing(ILogger<SikaraServerClass> _logger, CancellationToken token)
    {
        InstantiateConnection(_logger);
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    IUtpMessage received = await _utpClient.ReceiveMessageAsync(token);

                    _logger.LogInformation("🎉 Сообщение получено!");

                    _logger.LogInformation($"Результат: ActionCode: {(ActionCode)received.ActionCode}");
                    foreach (var header in received.Headers)
                        _logger.LogInformation($"{header.Key} : {header.Value}");

                    if (received is UtpMessage<JsonPayload> jsonMessage)
                    {
                        Console.WriteLine("JSON MESSAGE RECEIVED!");
                    }

                    if (received.PayloadStream is not null)
                    {
                        if (received.PayloadStream.CanSeek)
                            received.PayloadStream.Position = 0;

                        using var reader = new StreamReader(received.PayloadStream, Encoding.UTF8);
                        string payloadText = await reader.ReadToEndAsync();
                        //_logger.LogInformation($"{received.Headers["pType"]} - {payloadText}");
                        if (received is not null && payloadText.Length == int.Parse(received.Headers["pLen"]))
                            Console.WriteLine("CORRECT! THE RECEIVED PAYLOAD WAS DELIVERED WITHOUT EXTRA_CHANGES.");
                        else
                            Console.WriteLine("INCORRECT! THE PAYLOAD LENGTH DOES NOT CORRESPOND TO THE STATED IN HEADERS");
                    }
                    else
                    {
                        _logger.LogInformation("PayloadStream is null");
                    }
                }
                catch (EndOfStreamException)
                {
                    _logger.LogInformation("Client {ClientId} disconnected", Id);
                    break;
                }
                catch (IOException)
                {
                    _logger.LogError($"IOException occured while processing Client with ID: {Id}");
                    break;
                }
                catch (SocketException)
                {
                    _logger.LogError($"SocketException occured while processing Client with ID: {Id}");
                    break;
                }
            }
        }
        finally { CloseConnection(_logger); }
    }



    private void InstantiateConnection(ILogger<SikaraServerClass> _logger)
    {        
        _utpClient = new UtpClient(new UtpConnection(_tcpClient.Client));
        _logger.LogInformation("Connection with the Client ({ClientId} was instatntiated.", Id);
    }

    private void CloseConnection(ILogger<SikaraServerClass> _logger)
    {
        _tcpClient.Close();
        _logger.LogInformation("Connection with client {ClientId} closed.", Id);
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
            _tcpClient?.Close();
            _tcpClient?.Dispose();
        }
        _disposed = true;
    }
}
