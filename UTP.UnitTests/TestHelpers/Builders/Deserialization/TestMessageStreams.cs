using System.Buffers.Binary;
using System.Text.Json;
using UTP.UnitTests.Helpers.Payload;

namespace UTP.UnitTests.Helpers.Builders.Deserialization;

internal class TestMessageStreams
{
    public static IEnumerable<object[]> GetTestMessagesWithExpectations()
    {
        // 1.
        var headers1 = new Dictionary<string, string> { ["Content"] = "JSON" };
        var payload1 = new TestPayload(1, "Ok");
        yield return CreateTestCase(10, headers1, payload1, payload1.Name);

        // 2.
        var headers2 = new Dictionary<string, string>();
        var payload2 = new TestPayload(2, "NoHeaders");
        yield return CreateTestCase(20, headers2, payload2, payload2.Name);

        // 3. 
        var headers3 = new Dictionary<string, string> { ["Lang"] = "UA/RU", ["Icon"] = "🚀" };
        var payload3 = new TestPayload(3, "Привет!");
        yield return CreateTestCase(30, headers3, payload3, payload3.Name);

        // 4.
        var bigData = new string('x', 5000);
        var headers4 = new Dictionary<string, string> { ["Size"] = "Large" };
        var payload4 = new TestPayload(4, bigData);
        yield return CreateTestCase(40, headers4, payload4, payload4.Name);

        // 5. 
        var headers5 = new Dictionary<string, string> { ["Code"] = "Zero" };
        var payload5 = new TestPayload(5, "ZeroCode");
        yield return CreateTestCase(0, headers5, payload5, payload5.Name);

        // 6.
        var headers6 = new Dictionary<string, string> { ["Code"] = "Max" };
        var payload6 = new TestPayload(6, "MaxCode");
        yield return CreateTestCase(short.MaxValue, headers6, payload6, payload6.Name);

        // 7.
        var manyHeaders = Enumerable.Range(0, 100).ToDictionary(i => $"k{i}", i => $"v{i}");
        var payload7 = new TestPayload(7, "ManyHeaders");
        yield return CreateTestCase(70, manyHeaders, payload7, payload7.Name);

        // 8.
        var headers8 = new Dictionary<string, string> { ["Payload"] = "Empty" };
        yield return new object[] {
            CreateRawStream(80, headers8, new { }), 
            (short)80,
            JsonSerializer.SerializeToUtf8Bytes(headers8).Length,
            null!
        };

        // 9.
        var headers9 = new Dictionary<string, string> { ["Metadata"] = "{\"origin\":\"sensor-1\"}" };
        var payload9 = new TestPayload(9, "Nested");
        yield return CreateTestCase(90, headers9, payload9, payload9.Name);

        // 10.
        var headers10 = new Dictionary<string, string> { ["Type"] = "Negative" };
        var payload10 = new TestPayload(10, "NegativeCode");
        yield return CreateTestCase(-100, headers10, payload10, payload10.Name);
    }

    private static object[] CreateTestCase(
        short action, 
        Dictionary<string, string> headers, 
        TestPayload payload, 
        string? expectedName)
    => new object[] {
        CreateRawStream(action, headers, payload),
        action,
        JsonSerializer.SerializeToUtf8Bytes(headers).Length,
        expectedName!
    };


    private static Stream CreateRawStream(short actionCode, Dictionary<string, string> headers, object? payload)
    {
        byte[] headersBytes = JsonSerializer.SerializeToUtf8Bytes(headers);
        byte[] payloadBytes = JsonSerializer.SerializeToUtf8Bytes(payload);

        int headersLen = headersBytes.Length;
        int totalDataLen = sizeof(short) + sizeof(int) + headersLen + payloadBytes.Length;

        byte[] packet = new byte[sizeof(int) + totalDataLen];

        BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(0), totalDataLen);
        BinaryPrimitives.WriteInt16BigEndian(packet.AsSpan(4), actionCode);
        BinaryPrimitives.WriteInt32BigEndian(packet.AsSpan(6), headersLen);

        headersBytes.CopyTo(packet.AsSpan(10));
        payloadBytes.CopyTo(packet.AsSpan(10 + headersLen));

        return new MemoryStream(packet);
    }
}