using System.Buffers;
using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text.Json;
using UTP.Builders.Interfaces;
using UTP.Constants;
using UTP.Helpers;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders;

internal static class MessageDeserializeBuilder
{
    public static IMessageDeserializer<TPayload> For<TPayload>(NetworkStream networkStream) 
        where TPayload : IPayload
    {
        return new MessageDeserializer<TPayload>(networkStream);
    }
}


internal sealed class MessageDeserializer<TPayload> 
    : IMessageDeserializer<TPayload>, 
      IDisposable
        where TPayload : IPayload
{
    private NetworkStream _stream = null!;
    private byte[]? _rentedBuffer;
    private int _dataLength;

    private short _actionCode;
    private Dictionary<string, string>? _headers;
    private TPayload? _payload;

    public MessageDeserializer(NetworkStream networkStream)
    {
        SetStream(networkStream);
    }

    public void SetStream(NetworkStream networkStream) 
        => _stream = networkStream;

    public UtpMessage<TPayload> Build()
    {
        if (_headers == null || _payload == null)
            throw new InvalidOperationException("Message not fully deserialized");

        return new UtpMessage<TPayload>(_actionCode, _headers, _payload!);
    }

    public void Reset()
    {
        _actionCode = default;
        _headers = null;
        _payload = default;
        _rentedBuffer = null;
        _dataLength = default;
    }

    public IMessageDeserializer<TPayload> PrepareBuffer()
    {
        _dataLength = BinaryHelper.ReadIntFromStream(_stream);
        _rentedBuffer = ArrayPool<byte>.Shared.Rent(_dataLength);

        // Read the entire message into the rented buffer
        _stream.ReadExactly(_rentedBuffer.AsSpan(0, _dataLength));
        return this;
    }

    public IMessageDeserializer<TPayload> DeserializeActionCode()
    {
        if (!BufferPrepared)
            throw new InvalidOperationException("Buffer not prepared");

        ReadOnlySpan<byte> span = _rentedBuffer.AsSpan(0, _dataLength);
        _actionCode = BinaryPrimitives.ReadInt16BigEndian(span[..UtpMessageConstants.Sizes.ActionCode]);

        return this;
    }

    public IMessageDeserializer<TPayload> DeserializeHeaders()
    {
        if (!BufferPrepared)
            throw new InvalidOperationException("Buffer not prepared");

        ReadOnlySpan<byte> span = _rentedBuffer.AsSpan(0, _dataLength);
        int sepIndex = span.IndexOf(UtpMessageConstants.Delimiters.HeaderPayload);
        var headerSpan = GetHeaderSpan(span, sepIndex);
        _headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headerSpan);

        return this;
    }

    public IMessageDeserializer<TPayload> DeserializePayload()
    {
        if (!BufferPrepared)
            throw new InvalidOperationException("Buffer not prepared");

        ReadOnlySpan<byte> span = _rentedBuffer.AsSpan(0, _dataLength);
        int sepIndex = span.IndexOf(UtpMessageConstants.Delimiters.HeaderPayload);
        var payloadSpan = span[(sepIndex + 1)..];

        _payload = JsonSerializer.Deserialize<TPayload>(payloadSpan);

        return this;
    }

    public void Dispose()
    {
        if (BufferPrepared)
        {
            ArrayPool<byte>.Shared.Return(_rentedBuffer!);
            _rentedBuffer = null;
        }
    }
    private bool BufferPrepared
        => _rentedBuffer != null;

    private static ReadOnlySpan<byte> GetHeaderSpan(ReadOnlySpan<byte> span, int sepIndex)
        => span[UtpMessageConstants.Sizes.ActionCode..sepIndex];
}
