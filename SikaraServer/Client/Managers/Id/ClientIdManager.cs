namespace SilkaraServer.Client.Managers.Id;

internal record ClientIdManager(Guid ClientId) : IClientIdManager
{
    public Guid ClientId { get; set; } = ClientId;
}