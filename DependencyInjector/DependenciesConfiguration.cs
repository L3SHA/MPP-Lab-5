using System;
using System.Collections.Generic;

namespace DependencyInjector
{
    public class DependenciesConfiguration
    {
        public Dictionary<Type, List<DependencyInfo>> Dependencies { get; }

        public DependenciesConfiguration()
        {
            Dependencies = new Dictionary<Type, List<DependencyInfo>>();
        }

        public void Register<TDependency, TImplementation>(LifeTime lifetime = LifeTime.Transient,
            ImplementationName name = ImplementationName.None)
            where TDependency : class where TImplementation : TDependency
        {
            var implementationInfo = new DependencyInfo(typeof(TImplementation), lifetime, name);

            if (Dependencies.ContainsKey(typeof(TDependency)))
            {
                Dependencies[typeof(TDependency)].Add(implementationInfo);
            }
            else
            {
                Dependencies.Add(typeof(TDependency), new List<DependencyInfo>());
                Dependencies[typeof(TDependency)].Add(implementationInfo);
            }
        }

        public void Register(Type dependencyType, Type implementationType)
        {
            var implementationInfo =
                new DependencyInfo(implementationType, LifeTime.Transient, ImplementationName.None);

            if (Dependencies.ContainsKey(dependencyType))
            {
                Dependencies[dependencyType].Add(implementationInfo);
            }
            else
            {
                Dependencies.Add(dependencyType, new List<DependencyInfo>());
                Dependencies[dependencyType].Add(implementationInfo);
            }
        }
    }
}