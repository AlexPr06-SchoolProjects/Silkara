using UtpTypes.UtpMessageContext;

namespace SilkaraServer.Domain.States;

internal interface IState
{
    bool CanExecute(short actionCode);
    Task OnEnterStateAsync(IUtpContext ctx);
    Task OnExitStateAsync(IUtpContext ctx);
}

