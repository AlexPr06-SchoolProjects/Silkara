namespace UTP.UtpMessage.Interfaces;

public interface IBaseUtpMessage<TActionCode, THeaders, TPayload>
{
    TActionCode ActionCode { get; set; }
    THeaders Headers { get; set; }
    TPayload Payload { get; set; }
    void Clear();
}
