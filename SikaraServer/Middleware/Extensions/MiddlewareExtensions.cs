using UTP.UtpMessage;
using UtpTypes.PayloadTypes.Server;
using UtpTypes.UtpClientType;

namespace SilkaraServer.Middleware.Extensions;

internal static class MiddlewareExtensions
{
    public static async Task NotifyUser(this UtpClient? client, short code, string message = "")
    {
        if (client is not null)
            await client.SendMessageAsync(new UtpMessage<ServerResponsePaylaod>(
                code, 
                new Dictionary<string, string>(), 
                new ServerResponsePaylaod(message)));
    }
}