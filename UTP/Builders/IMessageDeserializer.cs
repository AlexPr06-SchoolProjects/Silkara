using System.Net.Sockets;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders;

internal interface IMessageDeserializer<T>
    where T : IPayload
{
    public IMessageDeserializer<T> DeserializeActionCode(UtpMessage<T> utpMessage, NetworkStream networkStream);
    public IMessageDeserializer<T> DeserializeMetadata(UtpMessage<T> utpMessage, NetworkStream networkStream, StreamReader streamReader);
    public IMessageDeserializer<T> DeserializePayloadStream(UtpMessage<T> utpMessage, NetworkStream networkStream);
    public UtpMessage<T> Build();
}
