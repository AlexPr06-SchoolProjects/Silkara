using System.Net.Sockets;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders.Interfaces;

internal interface IMessageSerializer<T>
    where T : IPayload
{
    public IMessageSerializer<T> SerializeActionCode(UtpMessage<T> utpMessage);
    public IMessageSerializer<T> SerializeHeaders(UtpMessage<T> utpMessage);
    public IMessageSerializer<T> SerializePayload(UtpMessage<T> utpMessage);
    public RawUtpMessage Build();
    public void Reset();
}
