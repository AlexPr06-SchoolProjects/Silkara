using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilkaraClient.Client;

Console.OutputEncoding = System.Text.Encoding.UTF8;

HostApplicationBuilder builder = new HostApplicationBuilder();

builder.Services.AddHostedService<SilkaraClientClass>();

using IHost host = builder.Build();
await host.RunAsync();
