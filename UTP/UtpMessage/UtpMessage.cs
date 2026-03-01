using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage.Interfaces;

namespace UTP.UtpMessage;

sealed public class UtpMessage<TPayload> : IUtpMessage<TPayload>
    where TPayload : IPayload
{
    internal const byte MESSAGE_LEN_LABEL_SIZE = UtpMessageConstants.MESSAGE_LEN_LABEL_SIZE;
    internal const byte MESSAGE_LEN_ACTION_CODE = UtpMessageConstants.MESSAGE_LEN_ACTION_CODE;
    public short ActionCode { get; init; }
    internal Dictionary<string, string> HeadersInternal { get; init; }
    public IReadOnlyDictionary<string, string> Headers => HeadersInternal;
    public TPayload? Payload { get; init; }

    public UtpMessage(
        short actionCode, 
        Dictionary<string, string> headers, 
        TPayload? payload) 
    {
        ActionCode = actionCode;
        HeadersInternal = headers;
        Payload = payload;
    }

    public UtpMessage() { }
}
