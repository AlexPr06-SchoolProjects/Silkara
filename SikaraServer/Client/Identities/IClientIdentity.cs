using Microsoft.Extensions.Logging;
using SilkaraServer.Server;
using SilkaraServer.Client.Managers.Id;

namespace SilkaraServer.Client.Identities;

internal interface IClientIdentity : IAsyncDisposable
{
   IClientIdManager IdManager { get; }
   Task Processing(ILogger<SikaraServerClass> logger, CancellationToken token);
}
