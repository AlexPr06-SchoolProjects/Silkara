using System.Net.Sockets;
using SilkaraServer.Client.Identities;
using SilkaraServer.Client.Managers.Id;

namespace SilkaraServer.Client.Factories;

internal class ClientFactory : IClientFactory
{
    public static ClientFactory Instance = new ClientFactory();
    private ClientFactory() { }
    public IClientIdentity Create(TcpClient tcpClient)
        => new ClientIdentity(tcpClient, new ClientIdManager(Guid.NewGuid()));
    
}

