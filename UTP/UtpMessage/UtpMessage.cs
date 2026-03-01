using UTP.Constants;
using UTP.Payload;
using UTP.UtpMessage.Interfaces;

namespace UTP.UtpMessage;

sealed public class UtpMessage<TPayload> : IUtpMessage<TPayload>
    where TPayload : IPayload
{
    internal const byte MESSAGE_LEN_LABEL_SIZE = UtpMessageConstants.Sizes.Label;
    internal const byte MESSAGE_LEN_ACTION_CODE = UtpMessageConstants.Sizes.ActionCode;

    private byte _freezeFlags = 0;
    private short _actionCode;
    private Dictionary<string, string> _headers = new(0);
    private TPayload? _payload;

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

    public IDictionary<string, string> Headers
    {
        get => _headers;
        set
        {
            if ((_freezeFlags & 2) != 0) 
                return;
            _headers = value is Dictionary<string, string> dict 
                ? dict 
                : new Dictionary<string, string>(value);
            _freezeFlags |= 2;
        }
    }
    public TPayload? Payload
    {
        get => _payload;
        set
        {
            if((_freezeFlags & 4) != 0) 
                return;
            _payload = value;
            _freezeFlags |= 4;
        }
    }

    public UtpMessage(
        short actionCode, 
        Dictionary<string, string> headers,
        TPayload payload)
    {
        ActionCode = actionCode;
        Headers = headers;
        Payload = payload;
    }

    public UtpMessage(
        short actionCode,
        Dictionary<string, string> headers
        )
    {
        ActionCode = actionCode;
        Headers = headers;
    }

    public UtpMessage() { }

    public void Clear()
    {
        _freezeFlags = 0;
        _actionCode = 0;
        _headers.Clear();
        _payload = default;
    }
}
