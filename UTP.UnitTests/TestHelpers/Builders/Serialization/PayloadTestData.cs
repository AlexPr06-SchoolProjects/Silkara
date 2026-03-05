using System.Text.Json;
using UTP.UnitTests.Helpers.Payload;

namespace UTP.UnitTests.Helpers.Builders.Serialization;

internal static class PayloadTestData
{
    public static IEnumerable<object[]> GetPayloadTestData()
    {
        var p1 = new TestPayload(1, "A");
        yield return new object[] { p1, JsonSerializer.SerializeToUtf8Bytes(p1) };

        var p2 = new TestPayload(0, "");
        yield return new object[] { p2, JsonSerializer.SerializeToUtf8Bytes(p2) };

        var p3 = new TestPayload(int.MaxValue, "Name with \"quotes\", \n new lines and emoji 🚀");
        yield return new object[] { p3, JsonSerializer.SerializeToUtf8Bytes(p3) };

        var p4 = new TestPayload(999, new string('X', 10000));
        yield return new object[] { p4, JsonSerializer.SerializeToUtf8Bytes(p4) };
    }
}
