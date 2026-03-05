using UTP.UnitTests.Helpers.Payload;
using UTP.UnitTests.TestHelpers.UtpEngine;
using UTP.UtpMessage;

namespace UTP.UnitTests;

public class UtpEngineTests
{
    [Theory]
    [MemberData(
        nameof(ComplexRoundTripData.GetComplexRoundTripData),
        MemberType = typeof(ComplexRoundTripData))]
    public void RoundTripTest_WriteAndThenReceiveTheSameMessage_ShouldBEIndempotent(
        short actionCode,
        Dictionary<string, string> headers,
        TestPayload payload)
    {
        MemoryStream sharedStream = new MemoryStream();

        var utpMessageSend = new UtpMessage<TestPayload>(actionCode, headers, payload);
        var utpMessageReceive = new UtpMessage<TestPayload>();

        UtpEngine utpEngineSend = new UtpEngine(sharedStream);
        UtpEngine utpEngineReceive = new UtpEngine(sharedStream);

        utpEngineSend.SendMessage(utpMessageSend);
        sharedStream.Position = 0;
        utpMessageReceive = utpEngineReceive.ReceiveMessage<TestPayload>();

        Assert.Equal(utpMessageSend.ActionCode, utpMessageReceive.ActionCode);

        Assert.Equal(utpMessageSend.Headers.Count, utpMessageReceive.Headers.Count);
        foreach( var header in utpMessageSend.Headers)
        {
            Assert.Equal(header.Value, utpMessageReceive.Headers[header.Key]);
        }

        Assert.NotNull(utpMessageReceive.Payload);
        Assert.Equal(utpMessageSend?.Payload?.Id, utpMessageReceive.Payload.Id);
        Assert.Equal(utpMessageSend?.Payload?.Name, utpMessageReceive.Payload.Name);
    }
}
