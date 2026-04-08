using UtpTypes.UtpMessageContext;

namespace SilkaraServer.Client.States;

public interface IState
{
    bool CanExecute(short actionCode);
    Task OnEnterStateAsync(IUtpContext ctx);
    Task OnExitStateAsync(IUtpContext ctx);
}

