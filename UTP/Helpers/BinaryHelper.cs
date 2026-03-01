using System.Net.Sockets;

namespace UTP.Helpers;

internal static class BinaryHelper
{
    public static int ConvertToInt(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToInt32(bytes, 0);
    }

    public static byte[] ReadBytes(int count, NetworkStream networkStream)
    {
        byte[] bytes = new byte[count];
        networkStream.ReadExactly(bytes, 0, count);
        return bytes;
    }

    public static short ConvertToShort(byte[] bytes)
    {
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);

        return BitConverter.ToInt16(bytes, 0);
    }
}
