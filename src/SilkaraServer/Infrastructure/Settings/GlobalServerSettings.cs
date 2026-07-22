namespace SilkaraServer.Infrastructure.Settings;

internal static class GlobalServerSettings
{
    public static string EnvironmentMode = Environment.GetEnvironmentVariable("SERVER_DOTNET_ENVIRONMENT") ?? "Development";
    public static string IpAddress = Environment.GetEnvironmentVariable("SERVER_IP_ADDRESS") ?? "127.0.0.1";
    public static int Port = Environment.GetEnvironmentVariable("SERVER_PORT") != null
        ? int.Parse(Environment.GetEnvironmentVariable("SERVER_PORT")!)
        : 8040;
    public static int MaxRequestsPerMinute = Environment.GetEnvironmentVariable("SERVER_MAX_REQUESTS_PER_MINUTE") != null
        ? int.Parse(Environment.GetEnvironmentVariable("SERVER_MAX_REQUESTS_PER_MINUTE")!)
        : 100;
}

