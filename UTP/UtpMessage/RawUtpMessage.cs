using System.Buffers.Binary;
using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage.Interfaces;

namespace UTP.UtpMessage;

internal class RawUtpMessage : IRawUtpMessage
{
    private readonly ReadOnlyMemory<byte> _fullPacket;
    public ReadOnlyMemory<byte> ActionCode { get; init; }
    public ReadOnlyMemory<byte> Headers { get; init; }
    public ReadOnlyMemory<byte> Payload { get; init; }
    public ReadOnlyMemory<byte> FullPacket => _fullPacket;

    public RawUtpMessage(
        ReadOnlyMemory<byte> actionCode, 
        ReadOnlyMemory<byte> headers, 
        ReadOnlyMemory<byte> payload)
    {
        ActionCode = actionCode;
        Headers = headers;
        Payload = payload;
    }

    public ReadOnlyMemory<byte> BuildFullPacket()
    {
        int totalSize = UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE + 
            ActionCode.Length + 
            Headers.Length + 
            UtpMessageConstants.MESSAGE_HEADERS_PAYLOAD_SEPARATOR_BYTES_LEN + 
            Payload.Length;

        byte[] fullPacket = new byte[totalSize];

        Span<byte> fullPacketSpan = fullPacket.AsSpan();

        AddLenSizeToSpan(fullPacketSpan, totalSize);
        AddActionCodeToSpan(fullPacketSpan);
        AddHeadersToSpan(fullPacketSpan);
        AddSeparatorToSpan(fullPacketSpan);
        AddPayloadToSpan(fullPacket);
        return fullPacket;
    }

    private void AddLenSizeToSpan(Span<byte> span, int totalSize)
    {
        int dataLength = totalSize - UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE;
        BinaryPrimitives.WriteInt32BigEndian(
            span.Slice(0, UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE), dataLength);
    }

    private void AddActionCodeToSpan(Span<byte> span)
    {
        ActionCode.Span.CopyTo(span.Slice(UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE));
    }

    private void AddHeadersToSpan(Span<byte> span)
    {
        Headers.Span.CopyTo(span.Slice(
            UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE + 
            ActionCode.Length)
        );
    }

    private void AddSeparatorToSpan(Span<byte> span)
    {
        span[UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE + 
            ActionCode.Length + 
            Headers.Length] = UtpMessageConstants.MESSAGE_HEADERS_PAYLOAD_SEPARATOR;
    }

    private void AddPayloadToSpan(Span<byte> span)
    {
        Payload.Span.CopyTo(span.Slice(
            UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE + 
            ActionCode.Length + 
            Headers.Length + 
            UtpMessageConstants.MESSAGE_HEADERS_PAYLOAD_SEPARATOR_BYTES_LEN)
        );
    }
}