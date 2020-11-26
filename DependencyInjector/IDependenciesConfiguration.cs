using System;

namespace DependencyInjector
{
    public interface IDependenciesConfiguration
    {
        void Register<TInterface, TImplementation>(LifeTime lifeTime);

        void Register(Type depInterface, Type depImpl, LifeTime lifeTime);
    }
}