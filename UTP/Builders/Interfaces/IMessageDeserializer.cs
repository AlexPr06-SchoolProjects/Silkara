using System.Net.Sockets;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders;

internal interface IMessageDeserializer<T> : IDisposable
    where T : IPayload
{
    public IMessageDeserializer<T> PrepareBuffer();
    public IMessageDeserializer<T> DeserializeActionCode();
    public IMessageDeserializer<T> DeserializeHeaders();
    public IMessageDeserializer<T> DeserializePayload();
    public UtpMessage<T> Build();
    public void Reset();
}
