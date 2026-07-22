using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilkaraServer.Presentation.Server;
using SilkaraServer.Application.Managers.Client;
using SilkaraServer.Infrastructure.Settings;
using SilkaraServer.Infrastructure.Database;
using StackExchange.Redis;

Console.OutputEncoding = System.Text.Encoding.UTF8;

HostApplicationBuilder builder = new HostApplicationBuilder(args);

const int shutdownTimeoutSeconds = 6;

#region Adding_Builder_Services

builder.Services.AddSingleton<IClientManager, ClientManager>();
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(shutdownTimeoutSeconds);
});
builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(RedisSettings.ConnectionString));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

builder.Services.AddSingleton<GlobalServerSetuper>();

#endregion

builder.Services.AddHostedService<SilkaraServerClass>();

using IHost host = builder.Build();
await host.RunAsync();
