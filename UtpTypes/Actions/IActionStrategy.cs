namespace UtpTypes.Actions;

public interface IActionStrategy
{
    ActionNums Action { get; }
    Task ExecuteAsync();
}
