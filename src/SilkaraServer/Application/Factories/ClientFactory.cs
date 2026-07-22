using System.Net.Sockets;
using SilkaraServer.Domain.Identities;
using SilkaraServer.Domain.Id;
using StackExchange.Redis;

namespace SilkaraServer.Application.Factories;

internal class ClientFactory : IClientFactory
{
    private static ClientFactory? _instance;
    private readonly IDatabase _redisDb;
    public static ClientFactory Instance => _instance ??= _instance ?? throw new Exception("ClientFactory is not initialized.");
    private ClientFactory(IDatabase redisDb)
    {
        _redisDb = redisDb;
    }

    public static void Setup(IDatabase redisDb)
    {
        _instance = new ClientFactory(redisDb);
    }
    public IClientIdentity Create(TcpClient tcpClient)
        => new ClientIdentity(tcpClient, new ClientIdManager(Guid.NewGuid()), _redisDb);

}

