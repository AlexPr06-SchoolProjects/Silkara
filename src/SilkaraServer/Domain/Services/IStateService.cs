using SilkaraServer.Domain.States;

namespace SilkaraServer.Domain.Services;

internal interface IStateService
{
    IState CurrentState { get; }
    void SetState(IState state);
}
