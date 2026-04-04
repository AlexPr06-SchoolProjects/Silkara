using UTP.Payload;

namespace UtpTypes.PayloadTypes;

public sealed class EmptyPayload : IPayload
{
    public static readonly EmptyPayload Instance = new();
    private EmptyPayload() { }
    private static readonly Stream EmptyStream = Stream.Null;
    public Stream GetStream()
        => EmptyStream;
}
