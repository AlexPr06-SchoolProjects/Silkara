namespace SilkaraServer.Infrastructure.Database;

public class DatabaseConfig
{
    public string ConnectionString { get;}
    private string DatabaseName { get; }
    private string DatabasePort { get; }
    private string DatabaseUser { get; }
    private string DatabasePassword { get; }

    public DatabaseConfig()
    {
        DatabaseName = Environment.GetEnvironmentVariable("POSTGRES_DB")!;
        DatabasePort = Environment.GetEnvironmentVariable("POSTGRES_PORT")!;
        DatabaseUser = File.ReadAllText("/run/secrets/db_user").Trim();
        DatabasePassword = File.ReadAllText("/run/secrets/db_password").Trim();

        ConnectionString = $"Host=db;" +
                       $"Port={DatabasePort};" +
                       $"Database={DatabaseName};" +
                       $"Username={DatabaseUser};" +
                       $"Password={DatabasePassword};" +
                       $"SSL Mode=Require;" +
                       $"Trust Server Certificate=true;";
    }
}
