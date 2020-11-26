using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using DependencyInjector;

namespace DependencyInjectorTest
{

    public class DirtProtection : IProtection
    {
        private string protectionDescription;

        public DirtProtection()
        {
            protectionDescription = "Protected from dirt";
        }

        public string GetProtectionDescription()
        {
            return protectionDescription;
        }
    }
    
    public class WaterProtection : IProtection
    {
        private string protectionDescription;

        public WaterProtection()
        {
            protectionDescription = "Protected from water";
        }

        public string GetProtectionDescription()
        {
            return protectionDescription;
        }
    }
    
    public interface IFootwear<TProtection> where TProtection: IProtection
    {
        string GetDescription();
    }
    
    public interface IProtection
    {
        string GetProtectionDescription();
    }
    
    public class Boots<TProtection> : IFootwear<TProtection> where TProtection : IProtection
    {
        private string description;
        private IProtection protection;

        public Boots(TProtection protection)
        {
            description = "Boots: ";
            this.protection = protection;
        }

        public string GetDescription()
        {
            return description + protection.GetProtectionDescription();
        }
    }
    
    public class TestClass1
    {
        public string ForTest()
        {
            return "created";
        }
    }

    public interface TestInterface1
    {
        string ForTest();
    }
    
    public class TestClass2 : TestInterface1
    {
        public string ForTest()
        {
            return "created";
        }
    }

    public class TestClass2_1 : TestInterface1
    {
        public string ForTest()
        {
            return "created";
        }
    }
    
    public interface TestInterface2
    {
        string Nested();
    }

    public class TestClass3 : TestInterface2
    {
        private string innerDependency;

        public string Nested()
        {
            return innerDependency;
        }
        
        public TestClass3(TestInterface1 testInterface1)
        {
            innerDependency = testInterface1.ForTest();
        }
    }

    public interface TestInterface3
    {
        
    }

    public class TestClass4 : TestInterface3
    {
        public TestClass4(TestInterface2 testInterface2)
        {
            
        }
    }
    
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAsSelfDependency()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<TestClass1, TestClass1>();
            var provider = new DependencyProvider(dependencies);
            Assert.AreEqual("created", provider.Resolve<TestClass1>().ForTest());
        }

        [Test]
        public void TestInterfaceDependency()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<TestInterface1, TestClass2>();
            var provider = new DependencyProvider(dependencies);
            Assert.AreEqual("created", provider.Resolve<TestInterface1>().ForTest());
        }

        [Test]
        public void TestEnumerableDependency()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<TestInterface1, TestClass2>();
            dependencies.Register<TestInterface1, TestClass2_1>();
            var provider = new DependencyProvider(dependencies);
            Assert.AreEqual(2, provider.Resolve<IEnumerable<TestInterface1>>().Count());
        }

        [Test]
        public void TestNestedDependencies()
        {
            var dependencies = new DependenciesConfiguration();
            dependencies.Register<TestInterface1, TestClass2>();
            dependencies.Register<TestInterface2, TestClass3>();
            var provider = new DependencyProvider(dependencies);
            Assert.AreEqual("created", provider.Resolve<TestInterface2>().Nested());
        }

        [Test]
        public void TestOpenGenerics()
        {
            var configuration = new DependenciesConfiguration();
            configuration.Register<IProtection, WaterProtection>();
            configuration.Register(typeof(IFootwear<>), typeof(Boots<>));
            var provider = new DependencyProvider(configuration);

            var boots = provider.Resolve<IFootwear<IProtection>>();
            var description = boots.GetDescription();

            Assert.AreEqual("Boots: Protected from water", description);
        }

        [Test]
        public void TestNamedDependencies()
        {
            var configuration = new DependenciesConfiguration();
            configuration.Register<IProtection, WaterProtection>(name: ImplementationName.First);
            configuration.Register<IProtection, DirtProtection>(name: ImplementationName.Second);
            var provider = new DependencyProvider(configuration);

            var protection = provider.Resolve<IProtection>(ImplementationName.Second);
            var description = protection.GetProtectionDescription();

            Assert.AreEqual("Protected from dirt", description);
        }
    }
}