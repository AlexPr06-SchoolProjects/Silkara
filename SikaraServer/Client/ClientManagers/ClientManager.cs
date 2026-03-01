using System.Collections.Concurrent;

namespace SikaraServer.Client.ClientManagers;

internal class ClientManager : IClientManager
{
    private ConcurrentDictionary<Guid, IClientIdentity> _clientIdentities = new();
    private int _activeClientsCount = 0;
    private bool _disposed = false;

    public void AddClient(IClientIdentity clientIdentity)
    {
        if(_clientIdentities.TryAdd(clientIdentity.Id, clientIdentity))
            Interlocked.Increment(ref _activeClientsCount);
    }
    
    public void RemoveClient(Guid clientId)
    {
        if(_clientIdentities.TryRemove(clientId, out IClientIdentity client))
            Interlocked.Decrement(ref _activeClientsCount);
        if (client is not null)
            client.Dispose();
    }
    
    public IClientIdentity? GetClient(Guid clientId)
    {
        _clientIdentities.TryGetValue(clientId, out IClientIdentity? clientIdentity);
        return clientIdentity;
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
            foreach (var id in _clientIdentities.Keys)
                if (_clientIdentities.TryRemove(id, out IClientIdentity? client) && client is not null)
                    client.Dispose();

            _clientIdentities.Clear();
        }

        _disposed = true;
    }
}
