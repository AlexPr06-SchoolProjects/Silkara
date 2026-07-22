namespace UtpTypes.Services;

sealed public class GlobalServiceLocator : ServiceLocatorBase
{
    public static readonly GlobalServiceLocator Instance = new();
    private GlobalServiceLocator() { }
}
