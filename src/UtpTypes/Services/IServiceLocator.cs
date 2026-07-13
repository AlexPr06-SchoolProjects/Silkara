namespace UtpTypes.Services;

public interface IServiceLocator
{
    void Register<TService>(TService serviceInstance) where TService : class;
    void Register<TService, TImplementation>(TImplementation serviceInstance)
        where TService : class
        where TImplementation : class, TService;
    T GetRequiredService<T>() where T : class;
}
