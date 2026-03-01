using System.Buffers.Binary;
using System.Net.Sockets;

namespace UTP.Helpers;

internal static class BinaryHelper
{
    public static void WriteToSpan(short value, Span<byte> destination)
        => BinaryPrimitives.WriteInt16BigEndian(destination, value);

    public static void WriteToSpan(int value, Span<byte> destination)
        => BinaryPrimitives.WriteInt32BigEndian(destination, (short)value);

    public static int ConvertToInt(Span<byte> bytes) 
        => BinaryPrimitives.ReadInt32BigEndian(bytes);

    public static short ConvertToShort(Span<byte> bytes) 
        => BinaryPrimitives.ReadInt16BigEndian(bytes);

    public static void ReadFromStream(NetworkStream networkStream, Span<byte> buffer)
        => networkStream.ReadExactly(buffer);


    public static int ReadIntFromStream(NetworkStream networkStream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        ReadFromStream(networkStream, buffer);
        return BinaryPrimitives.ReadInt32BigEndian(buffer);
    }
}
