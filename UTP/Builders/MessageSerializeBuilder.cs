using System.Net.Sockets;
using UTP.Payload;
using UTP.Helpers;
using UTP.UtpMessage;

namespace UTP.Builders;

internal class MessageSerializeBuilder<T> : IMessageSerializer<T>
    where T : IPayload
{
    private UtpMessage<T> _serialized;

    public MessageSerializeBuilder()
    {
        _utpMessage = new UtpMessage<T>();
    }

    public UtpMessage<T> Build()
    {
        return _utpMessage;
    }

    public void Reset()
    {
        _utpMessage = new UtpMessage<T>();
    }

    public byte[] SerializeActionCode(UtpMessage<T> utpMessage, NetworkStream networkStream)
    {
        throw new NotImplementedException();
    }

    public byte[] SerializeMetadata(UtpMessage<T> utpMessage, NetworkStream networkStream, StreamWriter streamWriter)
    {
        throw new NotImplementedException();
    }

    public byte[] SerializePayloadStream(UtpMessage<T> utpMessage, NetworkStream networkStream)
    {
        throw new NotImplementedException();
    }
}
