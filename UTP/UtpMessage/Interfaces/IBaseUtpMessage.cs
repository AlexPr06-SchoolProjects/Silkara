namespace UTP.UtpMessage.Interfaces;

public interface IBaseUtpMessage<TActionCode, THeaders, TPayload>
{
    TActionCode ActionCode { get; }
    THeaders Headers { get; }
    TPayload Payload { get; }
}
