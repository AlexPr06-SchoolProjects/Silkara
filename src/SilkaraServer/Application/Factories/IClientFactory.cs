using SilkaraServer.Domain.Identities;
using System.Net.Sockets;

namespace SilkaraServer.Application.Factories;

internal interface IClientFactory
{
    IClientIdentity Create(TcpClient tcpClient);
}
