namespace SilkaraServer.Client.Interfaces;

internal interface IClientManager : IAsyncDisposable
{
    void AddClient(IClientIdentity clientIdentity);
    Task RemoveClient(Guid clientId);
    IClientIdentity? GetClient(Guid clientId);
}
