using FluentAssertions;
using UTP.Builders;
using UTP.UnitTests.Helpers.Builders.Serialization;
using UTP.UnitTests.Helpers.Payload;
using UTP.UtpMessage;

namespace UTP.UnitTests.Builders;

public class MessageSerializeBuilderTests
{
    [Theory]
    [MemberData(
        nameof(ActionCodeTestDataClass.GetActionCodeTestData),
        MemberType = typeof(ActionCodeTestDataClass))]
    public void SerializeActionCode_CodeWasGiven_ReturnsCorrectResult(short code, byte[] expected)
    {
        UtpMessage<TestPayload> utpMessage = new UtpMessage<TestPayload>(
            actionCode: code, 
            headers: new Dictionary<string, string>());

        RawUtpMessage serializedMessage = MessageSerializeBuilder.For<TestPayload>()
            .SerializeActionCode(utpMessage)
            .Build();

        MessageSerializeBuilder.For<TestPayload>().Reset();
        serializedMessage.ActionCode.ToArray().Should().Equal(expected);
    }


    [Theory]
    [MemberData(
        nameof(HeadersTestDataClass.HeadersTestDataForSerialization), 
        MemberType = typeof(HeadersTestDataClass))]
    public void SerializeHeaders_VariousData_ReturnsCorrectBytes(
        Dictionary<string, string> givenHeaders, 
        byte[] expectedHeaders, 
        byte[] expectedLen)
    {
        UtpMessage<TestPayload> utpMessage = new UtpMessage<TestPayload>(
            actionCode: 10, 
            headers: givenHeaders);

        RawUtpMessage serializedMessage = MessageSerializeBuilder.For<TestPayload>()
            .SerializeHeaders(utpMessage)
            .Build();

        MessageSerializeBuilder.For<TestPayload>().Reset();
        serializedMessage.Headers.ToArray().Should().Equal(expectedHeaders);
        serializedMessage.HeadersLen.ToArray().Should().Equal(expectedLen);
    }

    [Theory]
    [MemberData(
        nameof(PayloadTestData.GetPayloadTestData),
        MemberType = typeof(PayloadTestData))]
    public void SerializePayload_VariousData_ReturnsCorrectBytes(
        TestPayload givenPayload, 
        byte[] expectedBytes)
    {
        UtpMessage<TestPayload> utpMessage = new UtpMessage<TestPayload>(
            actionCode: 10, 
            headers: new Dictionary<string, string>(),
            payload: givenPayload);

        RawUtpMessage serializedMessage = MessageSerializeBuilder.For<TestPayload>()
            .SerializePayload(utpMessage)
            .Build();

        MessageSerializeBuilder.For<TestPayload>().Reset();
        serializedMessage.Payload.ToArray().Should().Equal(expectedBytes);
    }
}