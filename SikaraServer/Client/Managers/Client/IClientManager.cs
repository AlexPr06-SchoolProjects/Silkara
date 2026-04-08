using SilkaraServer.Client.Identities;

namespace SilkaraServer.Client.Managers.Client;

internal interface IClientManager : IAsyncDisposable
{
    void AddClient(IClientIdentity clientIdentity);
    Task RemoveClient(Guid clientId);
    IClientIdentity? GetClient(Guid clientId);
}
