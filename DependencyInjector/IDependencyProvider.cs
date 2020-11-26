namespace DependencyInjector
{
    public interface IDependencyProvider
    {
        T Resolve<T>();
    }
}