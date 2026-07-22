using UTP.Visitors;
using UTP.UtpMessage;
using UTP.Payload;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Visitors;

public class ContextCreatorVisitor(CancellationToken ct = default) : IUtpMessageVisitor<UtpContext>
{
    public UtpContext Visit<TPayload>(UtpMessage<TPayload> message) where TPayload : IPayload
    {
        return new UtpContext(
            ct,
            message.ActionCode,
            (Dictionary<string, string>)message.Headers,
            message.Payload!);
    }
}
