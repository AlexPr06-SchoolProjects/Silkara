namespace UTP.UtpMessage.Interfaces;

public interface IRawUtpMessage:
    IBaseUtpMessage<short, ReadOnlyMemory<byte>, ReadOnlyMemory<byte>>
{
    public ReadOnlyMemory<byte> BuildFullPacket();
}
