using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilkaraClient.Client;

HostApplicationBuilder builder = new HostApplicationBuilder();

builder.Services.AddHostedService<SilkaraClientClass>();

using IHost host = builder.Build();
await host.RunAsync();
