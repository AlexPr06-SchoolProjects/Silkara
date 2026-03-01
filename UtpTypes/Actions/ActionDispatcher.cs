namespace UtpTypes.Actions;

public class ActionDispatcher
{
    private readonly Dictionary<ActionNums, IActionStrategy> _strategies;

    public ActionDispatcher(IEnumerable<IActionStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.Action);
    }

    public async Task DispatchAsync(ActionNums actionNum)
    {
        if (_strategies.TryGetValue(actionNum, out var strategy))
            await strategy.ExecuteAsync();
        else 
            throw new InvalidOperationException($"No strategy found for action number: {actionNum}");
    }
}
