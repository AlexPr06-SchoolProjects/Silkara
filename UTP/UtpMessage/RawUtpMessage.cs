using System.Buffers.Binary;
using UTP.Constants;
using UTP.UtpMessage.Interfaces;

namespace UTP.UtpMessage;

public class RawUtpMessage : IRawUtpMessage
{
    public ReadOnlyMemory<byte> ActionCode { get; set; }
    public ReadOnlyMemory<byte> HeadersLen { get; set; }
    public ReadOnlyMemory<byte> Headers { get; set; }
    public ReadOnlyMemory<byte> Payload { get; set; }
    public ReadOnlyMemory<byte> FullPacket { get; private set; }

    public RawUtpMessage(
        ReadOnlyMemory<byte> actionCode, 
        ReadOnlyMemory<byte> headers, 
        ReadOnlyMemory<byte> payload)
    {
        ActionCode = actionCode;
        Headers = headers;
        Payload = payload;
    }

    public RawUtpMessage() { }

    public void Clear()
    {
        ActionCode = ReadOnlyMemory<byte>.Empty;
        HeadersLen = ReadOnlyMemory<byte>.Empty;
        Headers = ReadOnlyMemory<byte>.Empty;
        Payload = ReadOnlyMemory<byte>.Empty;
        FullPacket = ReadOnlyMemory<byte>.Empty;
    }

    public ReadOnlyMemory<byte> BuildFullPacket()
    {
        int totalSize = 
            UtpConstants.Sizes.MessageLen +
            UtpConstants.Sizes.ActionCodeLen +
            UtpConstants.Sizes.HeadersLen +
            Headers.Length + 
            Payload.Length;

        byte[] fullPacket = new byte[totalSize];
        Span<byte> fullPacketSpan = fullPacket.AsSpan();

        int currentOffset = 0;
        int dataLength = totalSize - UtpConstants.Sizes.MessageLen;

        AddMessageLenToSpan(fullPacketSpan, currentOffset, dataLength);
        currentOffset += UtpConstants.Sizes.MessageLen;

        AddActionCodeToSpan(fullPacketSpan, currentOffset);
        currentOffset += UtpConstants.Sizes.ActionCodeLen;

        AddHeadersLenToSpan(fullPacketSpan, currentOffset, Headers.Length);
        currentOffset += UtpConstants.Sizes.HeadersLen;

        AddHeadersToSpan(fullPacketSpan, currentOffset);
        currentOffset += Headers.Length;

        AddPayloadToSpan(fullPacketSpan, currentOffset);
        currentOffset += Payload.Length;

        FullPacket = fullPacket;

        return FullPacket;
    }

    private void AddMessageLenToSpan(Span<byte> span, int startOffset, int length)
        => BinaryPrimitives.WriteInt32BigEndian(span.Slice(startOffset), length);

    private void AddActionCodeToSpan(Span<byte> span, int startOffset)
    => ActionCode.Span.CopyTo(span.Slice(startOffset));

    private void AddHeadersLenToSpan(Span<byte> span, int startOffset, int headersLength)
        => BinaryPrimitives.WriteInt32BigEndian(span.Slice(startOffset), headersLength);

    private void AddHeadersToSpan(Span<byte> span, int startOffset)
        => Headers.Span.CopyTo(span.Slice(startOffset));

    private void AddPayloadToSpan(Span<byte> span, int startOffset)
        => Payload.Span.CopyTo(span.Slice(startOffset));
}
