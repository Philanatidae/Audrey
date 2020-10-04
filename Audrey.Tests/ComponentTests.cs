using Audrey.Tests.Components;
using NUnit.Framework;

namespace Audrey.Tests
{
    [TestFixture]
    public class ComponentTests
    {
        [Test]
        public void ComponentAdd()
        {
            Engine engine = new Engine();
            Entity entity = engine.CreateEntity();

            Assert.NotNull(entity.AddComponent(new TestComponent1()));
        }
        
        [Test]
        public void ComponentGet()
        {
            Engine engine = new Engine();
            Entity entity = engine.CreateEntity();

            Assert.Null(entity.GetComponent<TestComponent1>());

            Assert.NotNull(entity.AddComponent(new TestComponent1()));

            Assert.NotNull(entity.GetComponent<TestComponent1>());
        }

        [Test]
        public void ComponentHas()
        {
            Engine engine = new Engine();
            Entity entity = engine.CreateEntity();

            Assert.IsFalse(entity.HasComponent<TestComponent1>());

            Assert.NotNull(entity.AddComponent(new TestComponent1()));

            Assert.IsTrue(entity.HasComponent<TestComponent1>());
        }

        [Test]
        public void ComponentRemove()
        {
            Engine engine = new Engine();
            Entity entity = engine.CreateEntity();

            entity.AddComponent(new TestComponent1());

            Assert.IsTrue(entity.HasComponent<TestComponent1>());

            entity.RemoveComponent<TestComponent1>();

            Assert.IsFalse(entity.HasComponent<TestComponent1>());
        }
    }
}
