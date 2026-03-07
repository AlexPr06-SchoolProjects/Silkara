namespace SilkaraServer.Client.Interfaces;

internal interface IClientManager : IDisposable
{
    void AddClient(IClientIdentity clientIdentity);
    void RemoveClient(Guid clientId);
    IClientIdentity? GetClient(Guid clientId);
}
