using SilkaraServer.Client.Identities;

namespace SilkaraServer.Client.Managers;

internal interface IClientManager : IAsyncDisposable
{
    void AddClient(IClientIdentity clientIdentity);
    Task RemoveClient(Guid clientId);
    IClientIdentity? GetClient(Guid clientId);
}
