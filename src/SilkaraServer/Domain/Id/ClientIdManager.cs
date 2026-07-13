namespace SilkaraServer.Domain.Id;

internal record ClientIdManager(Guid ClientId) : IClientIdManager
{
    public Guid ClientId { get; set; } = ClientId;
}