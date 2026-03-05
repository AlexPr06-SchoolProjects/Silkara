using System.Text.Json;
using UTP.Constants;

namespace UTP.UnitTests.Helpers.Builders.Serialization;

internal class HeadersTestDataClass
{
    public static IEnumerable<object[]> HeadersTestDataForSerialization()
    {
        var dict1 = new Dictionary<string, string>();
        byte[] bytes1 = JsonSerializer.SerializeToUtf8Bytes(dict1);
        yield return new object[] { dict1, bytes1, GetIntBytes(bytes1.Length) };

        var dict2 = new Dictionary<string, string> { ["auth"] = "secret" };
        byte[] bytes2 = JsonSerializer.SerializeToUtf8Bytes(dict2);
        yield return new object[] { dict2, bytes2, GetIntBytes(bytes2.Length) };

        var dict3 = new Dictionary<string, string>
        {
            [UtpConstants.Headers.PayloadTypeKey] = "Test",
            [UtpConstants.Headers.PayloadLenKey] = "10"
        };
        byte[] bytes3 = JsonSerializer.SerializeToUtf8Bytes(dict3);
        yield return new object[] { dict3, bytes3, GetIntBytes(bytes3.Length) };
    }

    private static byte[] GetIntBytes(int value)
    {
        byte[] b = new byte[4];
        System.Buffers.Binary.BinaryPrimitives.WriteInt32BigEndian(b, value);
        return b;
    }
}
