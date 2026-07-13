using SilkaraServer.Domain.States;
using SilkaraServer.Domain.States.StateConcretes;

namespace SilkaraServer.Domain.Services.StateServicesConcretes;

internal class ClientStateService : IStateService
{
    private IState _currentState = new UnauthorizedState();
    public IState CurrentState => _currentState;
    public void SetState(IState state)
    {
        // Here can be made additional logic (e.g. logging, etc.)
        _currentState = state;
    }
}

