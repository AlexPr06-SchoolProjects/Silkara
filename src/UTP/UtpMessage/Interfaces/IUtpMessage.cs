using UTP.Visitors;

namespace UTP.UtpMessage.Interfaces;

public interface IUtpMessage : IDisposable
{
    short ActionCode { get; }
    int HeadersLen { get; }
    IDictionary<string, string> Headers { get; }
    Stream? PayloadStream { get; }
    TResult Accept<TResult>(IUtpMessageVisitor<TResult> visitor);
}
