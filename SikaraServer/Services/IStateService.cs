using SilkaraServer.Client.States;

namespace SilkaraServer.Services;

public interface IStateService
{
    IState CurrentState { get; }
    void SetState(IState state);
}