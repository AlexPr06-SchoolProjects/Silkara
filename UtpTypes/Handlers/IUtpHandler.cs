using UtpTypes.UtpMessageContext;

namespace UtpTypes.Handlers;

public interface IUtpHandler
{
    short ActionCode { get; }
    Task HandleAsync(UtpContext ctx);
}
