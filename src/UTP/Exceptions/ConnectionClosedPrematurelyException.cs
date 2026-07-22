namespace UTP.Exceptions;

public class ConnectionClosedPrematurelyException : Exception
{
    public ConnectionClosedPrematurelyException()
        : base("The connection was closed prematurely while there were still bytes in the buffer.") { }
    public ConnectionClosedPrematurelyException(string message)
        : base(message) { }
    public ConnectionClosedPrematurelyException(string message, Exception innerException)
        : base(message, innerException) { }
}
