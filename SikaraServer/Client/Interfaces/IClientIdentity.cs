using Microsoft.Extensions.Logging;
using SikaraServer.App;

namespace SilkaraServer.Client.Interfaces;

internal interface IClientIdentity : IDisposable
{
    Guid Id { get; }
    void Processing(ILogger<SikaraServerClass> _logger);
}
