using System.Net.Sockets;
using UTP.Builders;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP;

public class UtpEngine(NetworkStream networkStream)
{
    public UtpMessage<TPayload> ReceiveMessage<TPayload>()
        where TPayload : IPayload
            => DeserializeMessage<TPayload>(networkStream);

    public void SendMessage<TPayload>(UtpMessage<TPayload> utpMessage)
        where TPayload : IPayload
    {
        RawUtpMessage serializedMessage = SerializeMessage(utpMessage);
        ReadOnlyMemory<byte> buffer = serializedMessage.BuildFullPacket();
        networkStream.Write(buffer.Span);
    }

    private static UtpMessage<TPayload> DeserializeMessage<TPayload>(NetworkStream networkStream)
        where TPayload : IPayload
    {
        using var deserializer = MessageDeserializeBuilder.For<TPayload>(networkStream);
        return deserializer
            .PrepareBuffer()
            .DeserializeActionCode()
            .DeserializeHeaders()
            .DeserializePayload()
            .Build();
    }

    private static RawUtpMessage SerializeMessage<TPayload>(UtpMessage<TPayload> utpMessage)
        where TPayload : IPayload
            => MessageSerializeBuilder.For<TPayload>()
                .SerializeActionCode(utpMessage)
                .SerializeHeaders(utpMessage)
                .SerializePayload(utpMessage)
                .Build();
}
