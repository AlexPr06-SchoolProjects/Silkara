namespace UTP.UtpMessage.Interfaces;

public interface IBaseUtpMessage<TActionCode, THeadersLength, THeaders, TPayload>
{
    TActionCode ActionCode { get; }
    THeadersLength HeadersLen { get; }
    THeaders Headers { get; }
    TPayload Payload { get; }
    void Clear();
}
