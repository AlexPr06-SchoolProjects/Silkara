using System.Net.Sockets;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders;

internal interface IMessageSerializer<T>
    where T : IPayload
{
    public byte[] SerializeActionCode(UtpMessage<T> utpMessage, NetworkStream networkStream);
    public byte[] SerializeMetadata(UtpMessage<T> utpMessage, NetworkStream networkStream, StreamWriter streamWriter);
    public byte[] SerializePayloadStream(UtpMessage<T> utpMessage, NetworkStream networkStream);
    public UtpMessage<T> Build();
}
