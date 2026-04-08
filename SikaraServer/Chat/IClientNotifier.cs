namespace SilkaraServer.Chat;

public interface IClientNotifier
{
    Task NotifyClientsAsync(string message);
}

