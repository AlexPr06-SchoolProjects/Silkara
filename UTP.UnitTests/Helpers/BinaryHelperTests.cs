using FluentAssertions;
using System.Buffers.Binary;
using UTP.Helpers;

namespace UTP.UnitTests.Helpers;

public class BinaryHelperTests
{
    [Fact]
    public void WriteToSpan_ValidValue_WritesBigEndianBytes()
    {
        // Arrange 
        short value = 258;
        byte[] buffer = new byte[2];
        Span<byte> destination = buffer.AsSpan();

        // Act
        BinaryHelper.WriteToSpan(value, destination);

        // Assert
        buffer.Should().Equal(new byte[] { 0x01, 0x02 });
    }

    [Fact]
    public void WriteToSpan_SpanTooSmall_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        short value = 258;
        byte[] buffer = new byte[1]; // Too small for a short

        Action action = () => BinaryHelper.WriteToSpan(value, buffer.AsSpan());

        // Act and Assert
        action.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("length");
    }

    [Theory]
    [InlineData(1, new byte[] { 0, 0, 0, 1 })]
    [InlineData(-1, new byte[] { 255, 255, 255, 255 })]
    [InlineData(int.MaxValue, new byte[] { 127, 255, 255, 255 })]
    public void WriteToSpan_VariousValues_WritesBigEndianBytes(int value, byte[] expected)
    {
        // Arrange
        byte[] buffer = new byte[4];

        // Act
        BinaryHelper.WriteToSpan(value, buffer.AsSpan());

        // Assert
        buffer.Should().Equal(expected, "number should be correctly converted to big-endian byte array");
    }

    [Theory]
    [InlineData(new byte[] { 0, 1 }, 1)]
    public void ConvertToShort_ValidBytes_ReturnsCorrectValue(byte[] bytes, short expected)
    {
        // AAA
        BinaryHelper.ConvertToShort(bytes)
            .Should().Be(expected, "byte array should be correctly converted to short");
    }

    [Theory]
    [InlineData(new byte[] { 0, 0, 0, 1 }, 1)]
    public void ConvertToInt_ValidBytes_ReturnsCorrectValue(byte[] bytes, int expected)
    {
        // AAA
        BinaryHelper.ConvertToInt(bytes)
            .Should().Be(expected, "byte array should be correctly converted to int");
    }

    [Fact]
    public void ReadFromStream_StreamEndsPrematurely_ThrowsEndOfStreamException()
    {
        // Arrange 
        var buffer = new byte[4];
        using MemoryStream ms = new MemoryStream(new byte[2]);

        // Act
        var action = () => BinaryHelper.ReadFromStream(ms, buffer);

        // Assert
        action.Should().Throw<EndOfStreamException>()
            .WithMessage("*end of the stream*");
    }

    [Fact]
    public void ReadIntFromStream_ValidStream_ReturnsExpectedInt()
    {
        // Arrange
        var expectedValue = 123456;
        byte[] data = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(data, expectedValue);

        using var ms = new MemoryStream(data);

        // Act
        var result = BinaryHelper.ReadIntFromStream(ms);

        // Assert
        result.Should().Be(expectedValue);
    }
}
