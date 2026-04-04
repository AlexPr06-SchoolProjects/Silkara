using UTP.Payload;

namespace UtpTypes.PayloadTypes;

public sealed class EmptyPayload : IPayload
{
    private static readonly Stream EmptyStream = Stream.Null;
    public Stream GetStream()
        => EmptyStream;
}
