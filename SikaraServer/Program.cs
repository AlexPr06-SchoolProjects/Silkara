using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilkaraServer.Server;
using SilkaraServer.Client.Managers.Client;

Console.OutputEncoding = System.Text.Encoding.UTF8;

HostApplicationBuilder builder = new HostApplicationBuilder(args);

const int shutdownTimeoutSeconds = 6;

#region Adding_Builder_Services

builder.Services.AddSingleton<IClientManager, ClientManager>();
builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(shutdownTimeoutSeconds);
});

#endregion

builder.Services.AddHostedService<SikaraServerClass>();

using IHost host = builder.Build();
await host.RunAsync();
