using System.Buffers.Binary;
using UTP.Constants;
using UTP.UtpMessage.Interfaces;

namespace UTP.UtpMessage;

internal class RawUtpMessage : IRawUtpMessage
{
    private byte _freezeFlags = 0;
    private short _actionCode;
    private ReadOnlyMemory<byte> _headers;
    private ReadOnlyMemory<byte> _payload;
    private ReadOnlyMemory<byte> _fullPacket;
    public short ActionCode {
        get => _actionCode; 
        set
        {
            if ((_freezeFlags & 1) != 0) 
                return;
            _actionCode = value;
            _freezeFlags |= 1;
        }
    }
    public ReadOnlyMemory<byte> Headers { 
        get => _headers;
        set
        {
            if ((_freezeFlags & 2) != 0) 
                return;
            _headers = value;
            _freezeFlags |= 2;
        } 
    }
    public ReadOnlyMemory<byte> Payload {
        get => _payload;
        set
        {
            if ((_freezeFlags & 4) != 0)
                return;
            _payload = value;
            _freezeFlags |= 4;
        }
    }
    public ReadOnlyMemory<byte> FullPacket => _fullPacket;

    public RawUtpMessage(
        short actionCode, 
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
        _freezeFlags = 0;
        _actionCode = 0;
        _headers = ReadOnlyMemory<byte>.Empty;
        _payload = ReadOnlyMemory<byte>.Empty;
        _fullPacket = ReadOnlyMemory<byte>.Empty;
    }

    public ReadOnlyMemory<byte> BuildFullPacket()
    {
        int totalSize = UtpMessageConstants.Sizes.Label +
            sizeof(short) + 
            Headers.Length + 
            UtpMessageConstants.Sizes.HeaderPayloadSeparator + 
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
        int dataLength = totalSize - UtpMessageConstants.Sizes.Label;
        BinaryPrimitives.WriteInt32BigEndian(
            span.Slice(0, UtpMessageConstants.Sizes.Label), dataLength);
    }

    private void AddActionCodeToSpan(Span<byte> span)
    {
        BinaryPrimitives.WriteInt16BigEndian(span.Slice(4, 2), _actionCode);
    }

    private void AddHeadersToSpan(Span<byte> span)
    {
        Headers.Span.CopyTo(span.Slice(
            UtpMessageConstants.Sizes.Label +
            UtpMessageConstants.Sizes.ActionCode
        );
    }

    private void AddSeparatorToSpan(Span<byte> span)
    {
        span[UtpMessageConstants.Sizes.Label +
            sizeof(short) + 
            Headers.Length] = UtpMessageConstants.Sizes.HeaderPayloadSeparator;
    }

    private void AddPayloadToSpan(Span<byte> span)
    {
        Payload.Span.CopyTo(span.Slice(
            UtpMessageConstants.Sizes.Label +
            sizeof(short) + 
            Headers.Length +
            UtpMessageConstants.Sizes.HeaderPayloadSeparator)
        );
    }
}