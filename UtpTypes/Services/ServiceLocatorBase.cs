namespace UtpTypes.Services;

public class ServiceLocatorBase : IServiceLocator
{
   private readonly Dictionary<Type, object> _services = new();

   public void Register<TService>(TService serviceInstance)
        where TService : class
    {
        if (!_services.TryAdd(typeof(TService), serviceInstance))
        {
            _services[typeof(TService)] = serviceInstance;
        }
    }

    public void Register<TService, TImplementation>(TImplementation serviceInstance)
        where TService : class
        where TImplementation : class, TService
    {
        Register<TService>(serviceInstance);
    }

    public void Register(Type serviceType, object serviceInstance)
    {
        if (!_services.TryAdd(serviceType, serviceInstance))
        {
            _services[serviceType] = serviceInstance;
        }
    }

    public TService GetRequiredService<TService>() where TService : class
    {
        if (_services.TryGetValue(typeof(TService), out var service))
        {
            return (TService)service;
        }
        throw new InvalidOperationException($"Service of type {typeof(TService).FullName} is not registered.");
    }
}