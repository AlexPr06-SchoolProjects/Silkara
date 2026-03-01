using System.Net.Sockets;

namespace SikaraServer.Client;

internal interface IClientFactory
{
    IClientIdentity Create(TcpClient tcpClient);
}
