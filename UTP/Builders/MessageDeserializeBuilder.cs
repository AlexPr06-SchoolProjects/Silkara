using System.Net.Sockets;
using UTP.Payload;
using UTP.Helpers;
using UTP.UtpMessage;

namespace UTP.Builders;

internal static class MessageDeserializeBuilder
{
    public static IMessageDeserializer<T> For<T>() where T : IPayload
    {
        return new MessageBuilder<T>();
    }
}


internal class MessageBuilder<T> : IMessageDeserializer<T>
        where T : IPayload
{
    private UtpMessage<T> _utpMessage;

    public MessageBuilder()
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

    public IMessageDeserializer<T> DeserializeActionCode(UtpMessage<T> utpMessage, NetworkStream networkStream)
    {
        short actionCode = BinaryHelper.ConvertToShort(BinaryHelper.ReadBytes(UtpMessage<T>.MESSAGE_LEN_ACTION_CODE, networkStream));
        utpMessage.ActionCode = actionCode;
        return this;
    }

    public IMessageDeserializer<T> DeserializeMetadata(UtpMessage<T> utpMessage, NetworkStream networkStream, StreamReader streamReader)
    {
        //TODO: Extract metadata from utpMessage and set it to _utpMessage
        return this;
    }

    public IMessageDeserializer<T> DeserializePayloadStream(UtpMessage<T> utpMessage, NetworkStream networkStream)
    {
        //TODO: Extract payload stream from utpMessage and set it to _utpMessage
        return this;
    }
}
