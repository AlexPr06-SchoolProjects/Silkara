using Microsoft.Extensions.Logging;
using SilkaraServer.Domain.Id;

namespace SilkaraServer.Domain.Identities;

internal interface IClientIdentity : IAsyncDisposable
{
   IClientIdManager IdManager { get; }
   Task Processing(ILogger<SilkaraServerIdenity> logger, CancellationToken token);
}
