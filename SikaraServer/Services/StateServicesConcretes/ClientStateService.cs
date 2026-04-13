using SilkaraServer.Client.States;
using SilkaraServer.Client.States.StateConcretes;

namespace SilkaraServer.Services.StateServicesConcretes;

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

