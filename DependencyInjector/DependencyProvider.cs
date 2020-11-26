using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DependencyInjector
{
    public class DependencyProvider
    {
        private readonly Dictionary<Type, List<DependencyInfo>> dependencies;

        public DependencyProvider(DependenciesConfiguration configuration)
        {
            dependencies = configuration.Dependencies;
        }

        public T Resolve<T>(ImplementationName implementationName = ImplementationName.None) where T : class
        {
            Type dependencyType = typeof(T);

            return (T) Resolve(dependencyType, implementationName);
        }

        private object Resolve(Type dependencyType, ImplementationName implementationName = ImplementationName.None)
        {
            if (dependencyType.IsGenericType && (dependencyType.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
            {
                return GetAllImplementations(dependencyType.GetGenericArguments()[0]);
            }

            if (dependencyType.IsGenericType && dependencies.ContainsKey(dependencyType.GetGenericTypeDefinition()))
            {
                var implementationType = dependencies[dependencyType.GetGenericTypeDefinition()][0].ImplementationType;
                implementationType = implementationType.MakeGenericType(dependencyType.GetGenericArguments());

                if (dependencyType.IsAssignableFrom(implementationType))
                {
                    return CreateInstance(implementationType);
                }
            }

            if (!dependencies.ContainsKey(dependencyType))
            {
                return null;
            }

            var implementationInfo = GetImplementationInfo(dependencyType, implementationName);

            if (!dependencyType.IsAssignableFrom(implementationInfo.ImplementationType))
            {
                return null;
            }

            if (implementationInfo.Lifetime == LifeTime.Transient)
            {
                return CreateInstance(implementationInfo.ImplementationType);
            }

            if (implementationInfo.Lifetime == LifeTime.Singleton)
            {
                return Singleton.GetInstance(implementationInfo.ImplementationType, CreateInstance);
            }

            return null;
        }

        private DependencyInfo GetImplementationInfo(Type dependencyType, ImplementationName name)
        {
            var implementations = dependencies[dependencyType];
            return implementations.Where(info => info.Name == name).First();
        }

        private IEnumerable GetAllImplementations(Type dependencyType)
        {
            List<DependencyInfo> implementations = dependencies[dependencyType];
            Type collectionType = typeof(List<>).MakeGenericType(dependencyType);
            IList instances = (IList) Activator.CreateInstance(collectionType);

            foreach (var implementation in implementations)
            {
                instances.Add(CreateInstance(implementation.ImplementationType));
            }

            return instances;
        }

        private object CreateInstance(Type type)
        {
            ConstructorInfo constructor = type.GetConstructors()[0];
            var parameters = new List<object>();

            foreach (var parameter in constructor.GetParameters())
            {
                var attribute = (AttributeDependencyKey) parameter.GetCustomAttribute(typeof(AttributeDependencyKey));

                if (attribute == null)
                {
                    parameters.Add(Resolve(parameter.ParameterType));
                }
                else
                {
                    parameters.Add(Resolve(parameter.ParameterType, attribute.ImplementationName));
                }
            }

            return Activator.CreateInstance(type, parameters.ToArray());
        }
    }
}