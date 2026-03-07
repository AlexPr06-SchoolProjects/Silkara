using SilkaraServer.Client.Interfaces;
using System.Net.Sockets;

namespace SikaraServer.Client.ClientIdentities.BasicClientIdentity;

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

