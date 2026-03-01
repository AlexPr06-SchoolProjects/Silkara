using UTP.Payload;

namespace UTP.UtpMessage.Interfaces;

public interface IRawUtpMessage:
    IBaseUtpMessage<ReadOnlyMemory<byte>, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{ }
