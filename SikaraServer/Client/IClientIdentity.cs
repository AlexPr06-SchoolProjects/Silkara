using Microsoft.Extensions.Logging;
using SikaraServer.App;

namespace SikaraServer.Client;

internal interface IClientIdentity : IDisposable
{
    Guid Id { get; }
    void Processing(ILogger<SikaraServerClass> _logger);
}
