using System;
using System.Collections.Generic;

namespace DependencyInjector
{
    public class DependencyInfo
    {
        public LifeTime Lifetime { get; }
        public Type ImplementationType { get; }
        public ImplementationName Name { get; }
        
        public DependencyInfo(Type type, LifeTime lifetime, ImplementationName name)
        {
            Lifetime = lifetime;
            ImplementationType = type;
            Name = name;
        }
    }
}