using Microsoft.Extensions.Logging;
using SilkaraServer.Server;

namespace SilkaraServer.Client.Identities;

internal interface IClientIdentity : IAsyncDisposable
{
   Guid Id { get; }
   Task Processing(ILogger<SikaraServerClass> logger, CancellationToken token);
}
