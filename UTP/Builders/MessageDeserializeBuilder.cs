using System.Buffers;
using System.Buffers.Binary;
using System.Text.Json;
using UTP.Builders.Interfaces;
using UTP.Constants;
using UTP.Helpers;
using UTP.Payload;
using UTP.UtpMessage;

namespace UTP.Builders;

internal static class MessageDeserializeBuilder
{
    public static IMessageDeserializer<TPayload> For<TPayload>(Stream stream) 
        where TPayload : IPayload
    {
        return new MessageDeserializer<TPayload>(stream);
    }
}


internal sealed class MessageDeserializer<TPayload>
    : IMessageDeserializer<TPayload>,
      IDisposable
        where TPayload : IPayload
{
    private Stream _stream = null!;
    private byte[]? _rentedBuffer;
    private int _dataLength;
    private int _headersLength;

    private int _currentOffest;

    private short _actionCode;
    private Dictionary<string, string> _headers = new();
    private TPayload? _payload;

    public MessageDeserializer(Stream stream)
    {
        SetStream(stream);
    }

    public void SetStream(Stream stream)
        => _stream = stream;

    public UtpMessage<TPayload> Build()
        => new UtpMessage<TPayload>(_actionCode, _headers, _payload!);

    public void Reset()
    {
        _stream = null!;
        _rentedBuffer = null;
        _dataLength = default;
        _headersLength = 0;

        _currentOffest = 0;

        _actionCode = default;
        _headers?.Clear();
        _payload = default;
    }

    public IMessageDeserializer<TPayload> PrepareBuffer()
    {
        _dataLength = DeserializeMessageLen();
        _rentedBuffer = ArrayPool<byte>.Shared.Rent(_dataLength);
        _stream.ReadExactly(_rentedBuffer.AsSpan(0, _dataLength));
        _currentOffest = 0;

        return this;
    }

    public int DeserializeMessageLen()
        => BinaryHelper.ReadIntFromStream(_stream);
    

    public IMessageDeserializer<TPayload> DeserializeActionCode()
    {
        ReturnIfBufferIsNotPrepared();

        ReadOnlySpan<byte> span = _rentedBuffer.AsSpan(_currentOffest, UtpConstants.Sizes.ActionCodeLen);
        _actionCode = BinaryPrimitives.ReadInt16BigEndian(span);
        _currentOffest += UtpConstants.Sizes.ActionCodeLen;

        return this;
    }

    public IMessageDeserializer<TPayload> DeserializeHeaders()
    {
        ReturnIfBufferIsNotPrepared();

        ReadOnlySpan<byte> lenSpan = _rentedBuffer.AsSpan(_currentOffest, UtpConstants.Sizes.HeadersLen);
        _headersLength = BinaryPrimitives.ReadInt32BigEndian(lenSpan);
        _currentOffest += UtpConstants.Sizes.HeadersLen;
        ReadOnlySpan<byte> headersSpan = _rentedBuffer.AsSpan(_currentOffest, _headersLength);
        _headers = GetHeaders(headersSpan);
        _currentOffest += _headersLength;

        return this;
    }

    public IMessageDeserializer<TPayload> DeserializePayload()
    {
        ReturnIfBufferIsNotPrepared();

        int remaining = _dataLength - _currentOffest;
        ReadOnlySpan<byte> payloadSpan = _rentedBuffer.AsSpan(_currentOffest, remaining);
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

    private void ReturnIfBufferIsNotPrepared()
    {
        //TODO: Implement logic if buffer is not prepared
        // ..
        // ..
        //..
        if (!BufferPrepared)
            throw new InvalidOperationException("Buffer not prepared");
    }

    private static Dictionary<string, string> GetHeaders(ReadOnlySpan<byte> span)
        => JsonSerializer.Deserialize<Dictionary<string, string>>(span)!;
}
