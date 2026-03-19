using Microsoft.Extensions.Logging;
using SikaraServer.App;

namespace SilkaraServer.Client.Interfaces;

internal interface IClientIdentity : IAsyncDisposable
{
    Guid Id { get; }
   Task Processing(ILogger<SikaraServerClass> _logger, CancellationToken token);
}
