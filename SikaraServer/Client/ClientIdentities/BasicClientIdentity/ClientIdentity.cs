using JsonManagerLib.Classes;
using JsonManagerLib.Enums;
using JsonManagerLib.Records;
using Microsoft.Extensions.Logging;
using SilkaraServer.Server;
using SilkaraServer.Client.Interfaces;
using System.Net.Sockets;
using System.Text;
using UTP.Connection;
using UTP.UtpMessage;
using UTP.UtpMessage.Interfaces;
using UtpTypes.Actions;
using UtpTypes.PayloadTypes;
using UtpTypes.UtpClientType;

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
            logger.LogError($"Failed to instantiate UtpClient for Client with ID: {Id}");
            return;
        }

        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    IUtpMessage received = await _utpClient.ReceiveMessageAsync(ct);

                    logger.LogInformation("🎉 Сообщение получено!");

                    logger.LogInformation($"Результат: ActionCode: {(ActionCode)received.ActionCode}");
                    foreach (var header in received.Headers)
                        logger.LogInformation($"{header.Key} : {header.Value}");

                    if (received is UtpMessage<JsonPayload>)
                    {
                        Console.WriteLine("JSON MESSAGE RECEIVED!");
                    }

                    if (received.PayloadStream is not null)
                    {
                        if (received.PayloadStream.CanSeek)
                            received.PayloadStream.Position = 0;

                        using var reader = new StreamReader(received.PayloadStream, Encoding.UTF8);
                        string payloadText = await reader.ReadToEndAsync(ct);
                        //_logger.LogInformation($"{received.Headers["pType"]} - {payloadText}");
                        if (payloadText.Length == int.Parse(received.Headers["pLen"]))
                            Console.WriteLine("CORRECT! THE RECEIVED PAYLOAD WAS DELIVERED WITHOUT EXTRA_CHANGES.");
                        else
                            Console.WriteLine(
                                "INCORRECT! THE PAYLOAD LENGTH DOES NOT CORRESPOND TO THE STATED IN HEADERS");
                    }
                    else
                    {
                        logger.LogInformation("PayloadStream is null");
                    }
                }
                catch (EndOfStreamException)
                {
                    logger.LogInformation("Client {ClientId} disconnected", Id);
                    break;
                }
                catch (InvalidOperationException ex)
                {
                    logger.LogWarning($"{ex.Message}. Client with ID: {Id}");
                    break;
                }
                catch (IOException ex)
                {
                    logger.LogWarning($"{ex.Message}. Client with ID: {Id}");
                    break;
                }
                catch (SocketException ex)
                {
                    logger.LogWarning($"{ex.Message}. Client with ID: {Id}");
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Unexpected error: {ex.Message}. Client with ID: {Id}");
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
