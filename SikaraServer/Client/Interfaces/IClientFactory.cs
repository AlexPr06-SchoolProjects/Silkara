using System.Net.Sockets;

namespace SilkaraServer.Client.Interfaces;

internal interface IClientFactory
{
    IClientIdentity Create(TcpClient tcpClient);
}
