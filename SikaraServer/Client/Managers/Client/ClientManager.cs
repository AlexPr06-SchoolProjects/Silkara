using System.Collections.Concurrent;
using SilkaraServer.Client.Identities;

namespace SilkaraServer.Client.Managers.Client;

internal class ClientManager : IClientManager
{
    private ConcurrentDictionary<Guid, IClientIdentity> _clientIdentities = new();
    private int _activeClientsCount;
    private bool _disposed;

    public void AddClient(IClientIdentity clientIdentity)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ClientManager));

        if (_clientIdentities.TryAdd(clientIdentity.IdManager.ClientId, clientIdentity))
            Interlocked.Increment(ref _activeClientsCount);
    }
    
    public async Task RemoveClient(Guid clientId)
    {
        if(_clientIdentities.TryRemove(clientId, out IClientIdentity? client))
            Interlocked.Decrement(ref _activeClientsCount);
        if (client is not null)
            await client.DisposeAsync();
    }
    
    public IClientIdentity? GetClient(Guid clientId)
    {
        _clientIdentities.TryGetValue(clientId, out IClientIdentity? clientIdentity);
        return clientIdentity;
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
            var tasks = new List<Task>();
            foreach (var client in _clientIdentities.Values)
                tasks.Add(client.DisposeAsync().AsTask());

            _clientIdentities.Clear();

            await Task.WhenAll(tasks);
        }

        _disposed = true;
    }
}
