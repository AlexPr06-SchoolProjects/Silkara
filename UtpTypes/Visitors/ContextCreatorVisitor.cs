using UTP.Visitors;
using UTP.UtpMessage;
using UTP.Payload;
using UtpTypes.UtpMessageContext;

namespace UtpTypes.Visitors;

public class ContextCreatorVisitor(Guid clientId, CancellationToken ct = default) : IUtpMessageVisitor<UtpContext>
{
    public UtpContext Visit<TPayload>(UtpMessage<TPayload> message) where TPayload : IPayload
    {
        return new UtpContext(
            ct,
            clientId,
            message.ActionCode, 
            (Dictionary<string, string>)message.Headers, 
            message.Payload!);
    }
}