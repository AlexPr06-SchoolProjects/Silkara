namespace SilkaraServer.Chat;

internal interface IChat
{
    void AddClient(Guid clientId);
    void RemoveClient(Guid clientId);
    void AddClients(IEnumerable<Guid> clientIds);
    void RemoveClients(IEnumerable<Guid> clientIds);
}
