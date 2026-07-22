namespace UTP.Exceptions;

public class RemotePeerDisconnectedException : Exception
{
    public RemotePeerDisconnectedException()
        : base("The remote peer has disconnected.") { }
    public RemotePeerDisconnectedException(string message)
        : base(message) { }
    public RemotePeerDisconnectedException(string message, Exception innerException)
        : base(message, innerException) { }
}
