using System.Buffers.Binary;

namespace UTP.Helpers;

internal static class BinaryHelper
{
    public static void WriteToSpan(int value, Span<byte> destination)
        => BinaryPrimitives.WriteInt32BigEndian(destination, value);

    public static void WriteToSpan(short value, Span<byte> destination)
        => BinaryPrimitives.WriteInt16BigEndian(destination, value);

    public static int GetIntFromSpan(ReadOnlySpan<byte> span)
        => BinaryPrimitives.ReadInt32BigEndian(span);

    public static short GetShortFromSpan(ReadOnlySpan<byte> span)
        => BinaryPrimitives.ReadInt16BigEndian(span);

    public static int ConvertToInt(Span<byte> bytes)
        => BinaryPrimitives.ReadInt32BigEndian(bytes);

    public static short ConvertToShort(Span<byte> bytes)
        => BinaryPrimitives.ReadInt16BigEndian(bytes);

    public static void ReadFromStream(Stream stream, Span<byte> buffer)
        => stream.ReadExactly(buffer);

    public static int ReadIntFromStream(Stream stream)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        ReadFromStream(stream, buffer);
        return BinaryPrimitives.ReadInt32BigEndian(buffer);
    }
}
