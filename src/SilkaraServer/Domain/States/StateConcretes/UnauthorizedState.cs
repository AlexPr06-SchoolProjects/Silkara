using UtpTypes.UtpMessageContext;

namespace SilkaraServer.Domain.States.StateConcretes;

//TODO: Define logic for this state
internal class UnauthorizedState : IState
{
    public bool CanExecute(short actionCode)
    {
        return true;
    }

    public Task OnEnterStateAsync(IUtpContext ctx)
    {
        throw new NotImplementedException();
    }

    public Task OnExitStateAsync(IUtpContext ctx)
    {
        throw new NotImplementedException();
    }
}

