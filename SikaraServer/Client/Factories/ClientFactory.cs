using System.Net.Sockets;
using SilkaraServer.Client.Identities;

namespace SilkaraServer.Client.Factories;

internal class ClientFactory : IClientFactory
{
    public static ClientFactory Instance = new ClientFactory();
    private ClientFactory() { }
    public IClientIdentity Create(TcpClient tcpClient)
    {
        Guid newId = Guid.NewGuid();
        return new ClientIdentity(tcpClient, newId);
    }
}

