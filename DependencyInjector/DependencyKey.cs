using System;

namespace DependencyInjector
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class AttributeDependencyKey : Attribute
    {
        public ImplementationName ImplementationName { get; }

        public AttributeDependencyKey(ImplementationName name)
        {
            ImplementationName = name;
        }
    }
}