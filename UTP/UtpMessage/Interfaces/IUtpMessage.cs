namespace UTP.UtpMessage.Interfaces;

public interface IUtpMessage
{
    short ActionCode { get; }
    int HeadersLen { get; }
    IDictionary<string, string> Headers { get; }
    Stream? PayloadStream { get; }
}

