namespace SilkaraServer.Infrastructure.Settings;

internal static class RedisSettings
{
    private static string Host => Environment.GetEnvironmentVariable("REDIS_HOST") ?? "localhost";
    private static string Port => Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
    private static string Password => Environment.GetEnvironmentVariable("REDIS_PASSWORD") ?? "";
    public static string ConnectionString => $"{Host}:{Port},password={Password}";
}
