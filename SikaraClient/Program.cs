using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = new HostApplicationBuilder();

builder.Services.AddHostedService<SikaraClientClass>();

using IHost host = builder.Build();
await host.RunAsync();