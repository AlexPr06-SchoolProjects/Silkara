namespace SilkaraServer.Client.Managers.Id;

public record ClientIdManager(Guid ClientId) : IClientIdManager
{
    public Guid ClientId { get; set; } = ClientId;
}