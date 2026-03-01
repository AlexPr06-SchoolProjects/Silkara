using System.Net.Sockets;
using UTP.Builders;
using UTP.Payload;
using UTP.UtpMessage;
using UTP.UtpMessage.Interfaces;

namespace UTP;

public class UtpEngine
{
    private NetworkStream networkStream;
    private MemoryStream memoryStream = null!;

    public UtpEngine(NetworkStream netStream)
    {
        this.networkStream = netStream;
    }

    public UtpMessage<T> ReceiveMessage<T>()
        where T : IPayload
        => DeserializeMessage<T>(networkStream);

    public void SendMessage<T>(UtpMessage<T> utpMessage)
        where T : IPayload
    {
        IRawUtpMessage serializedMessage = SerializeMessage(utpMessage);
        ReadOnlyMemory<byte> buffer = serializedMessage.BuildFullPacket();
        networkStream.Write(buffer.Span);
    }

    private UtpMessage<TPayload> DeserializeMessage<TPayload>(NetworkStream networkStream)
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

    private IRawUtpMessage SerializeMessage<T>(UtpMessage<T> utpMessage)
        where T : IPayload
        => MessageSerializeBuilder.For<T>()
                .SerializeActionCode(utpMessage)
                .SerializeHeaders(utpMessage)
                .SerializePayload(utpMessage)
                .Build();
}
