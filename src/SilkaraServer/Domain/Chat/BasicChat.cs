namespace SilkaraServer.Domain.Chat;

internal class BasicChat : IChat, IClientNotifier
{
    private List<Guid> _clients = new List<Guid>();

    public void AddClient(Guid clientId)
    {
        _clients.Add(clientId);
    }

    public void RemoveClient(Guid clientId)
    {
        _clients.Remove(clientId);
    }

    public void AddClients(IEnumerable<Guid> clientIds)
    {
        _clients.AddRange(clientIds);
    }

    public void RemoveClients(IEnumerable<Guid> clientIds)
    {
        foreach (var clientId in clientIds)
        {
            _clients.Remove(clientId);
        }
    }

    public async Task NotifyClientsAsync(string message)
    {
        foreach (var client in _clients)
        {
            await SendMessageAsync(client, message);
        }
    }

    public async Task SendMessageAsync(Guid clientId, string message)
    {
        // TODO: send message to client
    }
}
