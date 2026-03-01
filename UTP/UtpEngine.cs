using System.Net.Sockets;
using UTP.Builders;
using UTP.Payload;
using UTP.Helpers;
using UTP.UtpMessage;

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
    {
        int readingSize = BinaryHelper.ConvertToInt(BinaryHelper.ReadBytes(UtpMessage<T>.MESSAGE_LEN_LABEL_SIZE, networkStream));

        memoryStream = new MemoryStream(readingSize);

        // memStream.Write(ReadBytes(readingSize), 0, readingSize);
        // TODO: ??? DEBUG
        memoryStream.SetLength(readingSize);
        networkStream.ReadExactly(memoryStream.GetBuffer(), 0, readingSize);
        memoryStream.Position = 0;

        UtpMessage<T> utpm = new UtpMessage<T>();

        using StreamReader sr = new StreamReader(memoryStream);

        //ExtractActionCode(utpm);
        //ExtractMetadata(utpm, sr);



        return utpm;

    }

    private byte[] SerializeMessage<T>(UtpMessage<T> utpMessage)
        where T : IPayload
    {
        //TODO: Implement serializeLogic
        return new byte[100];
    }

    private UtpMessage<T> DeserializeMessage<T>(UtpMessage<T> utpMessage)
        where T : IPayload =>
        MessageDeserializeBuilder.For<T>()
           .ExtractActionCode(utpMessage, networkStream)
           .ExtractMetadata(utpMessage, networkStream, new StreamReader(memoryStream))
           .ExtractPayloadStream(utpMessage, networkStream)
           .Build();
}
