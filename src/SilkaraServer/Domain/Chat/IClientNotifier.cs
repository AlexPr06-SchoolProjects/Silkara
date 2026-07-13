namespace SilkaraServer.Domain.Chat;

internal interface IClientNotifier
{
    Task NotifyClientsAsync(string message);
}

