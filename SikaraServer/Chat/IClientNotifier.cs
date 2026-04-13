namespace SilkaraServer.Chat;

internal interface IClientNotifier
{
    Task NotifyClientsAsync(string message);
}

