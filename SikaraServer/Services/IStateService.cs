using SilkaraServer.Client.States;

namespace SilkaraServer.Services;

internal interface IStateService
{
    IState CurrentState { get; }
    void SetState(IState state);
}