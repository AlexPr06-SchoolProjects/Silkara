using SilkaraServer.Client.Identities;
using System.Net.Sockets;

namespace SilkaraServer.Client.Factories;

internal interface IClientFactory
{
    IClientIdentity Create(TcpClient tcpClient);
}
