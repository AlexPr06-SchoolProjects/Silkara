namespace SilkaraClient.Settings;

internal static class GlobalClientSettings
{
    public static string IpAddress = Environment.GetEnvironmentVariable("SERVER_IP_ADDRESS_TO_CONNECT") ?? "127.0.0.1";
    public static int Port = Environment.GetEnvironmentVariable("SERVER_PORT_TO_CONNECT") != null 
        ? int.Parse(Environment.GetEnvironmentVariable("SERVER_PORT_TO_CONNECT")!) 
        : 8040;
}