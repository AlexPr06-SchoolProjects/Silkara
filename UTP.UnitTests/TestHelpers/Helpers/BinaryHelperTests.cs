using FluentAssertions;
using System.Buffers.Binary;
using UTP.Helpers;

namespace UTP.UnitTests.TestHelpers.Helpers;

public class BinaryHelperTests
{
    [Fact]
    public void WriteToSpan_ValidValue_WritesBigEndianBytes()
    {
        short value = 258;
        byte[] buffer = new byte[2];
        Span<byte> destination = buffer.AsSpan();
        BinaryHelper.WriteToSpan(value, destination);
        buffer.Should().Equal(new byte[] { 0x01, 0x02 });
    }

    [Fact]
    public void WriteToSpan_SpanTooSmall_ThrowsArgumentOutOfRangeException()
    {
        short value = 258;
        byte[] buffer = new byte[1]; // Too small for a short
        Action action = () => BinaryHelper.WriteToSpan(value, buffer.AsSpan());
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("length");
    }

    [Theory]
    [InlineData(0, new byte[] { 0, 0, 0, 0 })]
    [InlineData(255, new byte[] { 0, 0, 0, 255 })]
    [InlineData(256, new byte[] { 0, 0, 1, 0 })]
    [InlineData(int.MinValue, new byte[] { 128, 0, 0, 0 })]
    [InlineData(65535, new byte[] { 0, 0, 255, 255 })]
    public void WriteToSpan_VariousValues_WritesBigEndianBytes(int value, byte[] expected)
    {
        byte[] buffer = new byte[4];
        BinaryHelper.WriteToSpan(value, buffer.AsSpan());
        buffer.Should().Equal(expected, "number should be correctly converted to big-endian byte array");
    }

    [Theory]
    [InlineData(new byte[] { 0, 0 }, 0)]
    [InlineData(new byte[] { 0, 255 }, 255)]
    [InlineData(new byte[] { 255, 255 }, -1)]
    [InlineData(new byte[] { 127, 255 }, 32767)]
    [InlineData(new byte[] { 128, 0 }, -32768)]
    [InlineData(new byte[] { 0x01, 0x02 }, 258)]
    public void GetShortFromSpan_ValidData_ReturnsShort(byte[] span, short expected)
    {
        short result = BinaryHelper.GetShortFromSpan(span);
        result.Should().Be(expected, "Does not return correct value");
    }

    [Theory]
    [InlineData(new byte[] { 0, 0, 0, 0 }, 0)]
    [InlineData(new byte[] { 255, 255, 255, 255 }, -1)]
    [InlineData(new byte[] { 0, 0, 1, 0 }, 256)]
    [InlineData(new byte[] { 128, 0, 0, 0 }, int.MinValue)]
    [InlineData(new byte[] { 0, 0, 255, 255 }, 65535)]
    public void GetIntFromSpan_ValidData_ReturnsInt(byte[] span, int expected)
    {
        int result = BinaryHelper.GetIntFromSpan(span);
        result.Should().Be(expected, "Does not return correct value");
    }

    [Theory]
    [InlineData(new byte[] { 0, 0 }, 0)]
    [InlineData(new byte[] { 0, 255 }, 255)]
    [InlineData(new byte[] { 255, 255 }, -1)]
    [InlineData(new byte[] { 127, 255 }, 32767)]
    [InlineData(new byte[] { 128, 0 }, -32768)]
    [InlineData(new byte[] { 0x01, 0x02 }, 258)]
    public void ConvertToShort_ValidBytes_ReturnsCorrectValue(byte[] bytes, short expected)
    {
        BinaryHelper.ConvertToShort(bytes)
            .Should().Be(expected, "byte array should be correctly converted to short");
    }

    [Theory]
    [InlineData(new byte[] { 0, 0, 0, 0 }, 0)]
    [InlineData(new byte[] { 255, 255, 255, 255 }, -1)]
    [InlineData(new byte[] { 0, 0, 1, 0 }, 256)]
    [InlineData(new byte[] { 128, 0, 0, 0 }, int.MinValue)]
    [InlineData(new byte[] { 0, 0, 255, 255 }, 65535)]
    public void ConvertToInt_ValidBytes_ReturnsCorrectValue(byte[] bytes, int expected)
    {
        BinaryHelper.ConvertToInt(bytes)
            .Should().Be(expected, "byte array should be correctly converted to int");
    }

    [Fact]
    public void ReadFromStream_StreamEndsPrematurely_ThrowsEndOfStreamException()
    {
        var buffer = new byte[4];
        using MemoryStream ms = new MemoryStream(new byte[2]);
        var action = () => BinaryHelper.ReadFromStream(ms, buffer);
        action.Should().Throw<EndOfStreamException>()
            .WithMessage("*end of the stream*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(65536)] // 0x00010000
    public void ReadIntFromStream_VariousValues_ReturnsExpectedInt(int expectedValue)
    {
        byte[] data = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(data, expectedValue);
        using var ms = new MemoryStream(data);
        var result = BinaryHelper.ReadIntFromStream(ms);
        result.Should().Be(expectedValue);
    }
}
