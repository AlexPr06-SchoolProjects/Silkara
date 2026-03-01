using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SikaraClient.Client;


HostApplicationBuilder builder = new HostApplicationBuilder(args);

builder.Services.AddHostedService<SikaraClientClass>();

using IHost host = builder.Build();
await host.RunAsync();
