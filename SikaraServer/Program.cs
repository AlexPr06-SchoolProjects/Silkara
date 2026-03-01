using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SikaraServer.App;
using SikaraServer.Client;
using SikaraServer.Client.ClientManagers;

HostApplicationBuilder builder = new HostApplicationBuilder(args);


#region Adding_Builder_Services

builder.Services.AddSingleton<IClientManager, ClientManager>();

#endregion


builder.Services.AddHostedService<SikaraServerClass>();

using IHost host = builder.Build();
await host.RunAsync();
