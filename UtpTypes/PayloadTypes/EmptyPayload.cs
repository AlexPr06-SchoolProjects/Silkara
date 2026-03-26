using UTP.Payload;

namespace UtpTypes.PayloadTypes;

public sealed class EmptyPayload : IPayload
{
    private static readonly Stream _emptyStream = Stream.Null;
    public Stream GetStream()
        => _emptyStream;
}
