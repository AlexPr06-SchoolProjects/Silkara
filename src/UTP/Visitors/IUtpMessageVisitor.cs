using UTP.UtpMessage;
using UTP.Payload;

namespace UTP.Visitors;

public interface IUtpMessageVisitor<out TResult> : IUtpVisitor
{
    TResult Visit<TPayload>(UtpMessage<TPayload> message)
        where TPayload : IPayload;
}
