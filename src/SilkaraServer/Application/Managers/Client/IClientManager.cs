using SilkaraServer.Domain.Identities;

namespace SilkaraServer.Application.Managers.Client;

internal interface IClientManager : IAsyncDisposable
{
    void AddClient(IClientIdentity clientIdentity);
    Task RemoveClient(Guid clientId);
    IClientIdentity? GetClient(Guid clientId);
}
