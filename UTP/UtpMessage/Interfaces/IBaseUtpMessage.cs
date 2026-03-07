namespace UTP.UtpMessage.Interfaces;

public interface IBaseUtpMessage<TActionCode, THeadersLength, THeaders, TPayloadStream>
{
    TActionCode ActionCode { get; }
    THeadersLength HeadersLen { get; }
    THeaders Headers { get; }
    TPayloadStream? PayloadStream { get; }
    void Clear();
}
