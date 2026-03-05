using FluentAssertions;
using UTP.Builders;
using UTP.UnitTests.Helpers.Builders.Deserialization;
using UTP.UnitTests.Helpers.Payload;
using UTP.UtpMessage;

namespace UTP.UnitTests.Builders;

public class MessageDeserializeBuilderTests
{
    [Theory]
    [MemberData(
        nameof(TestMessageStreams.GetTestMessagesWithExpectations),
        MemberType = typeof(TestMessageStreams))]
    public void DeserializeTheWHoleStream_VariousStreamsOfBytesProvided_ShouldReturnCorrectData(
        Stream testStream,
        short expectedAction,
        int expectedHeaderCount,
        string expectedName)
    {
        using var deserializer = MessageDeserializeBuilder.For<TestPayload>(testStream);

        UtpMessage<TestPayload> utpMessage = deserializer
            .PrepareBuffer()
            .DeserializeActionCode()
            .DeserializeHeaders()
            .DeserializePayload()
            .Build();

        Assert.Equal(expectedAction, utpMessage.ActionCode);
        Assert.Equal(expectedHeaderCount, utpMessage.HeadersLen);
        Assert.Equal(expectedName, utpMessage?.Payload?.Name);
    }
}
