using Audrey.Tests.Components;
using NUnit.Framework;

namespace Audrey.Tests
{
    [TestFixture]
    public class EntityTests
    {
        [Test]
        public void EntityCreation()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();
            Assert.IsNotNull(entity);
        } 

        [Test]
        public void EntityDestruction()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();
            Assert.IsTrue(entity.IsValid());

            engine.DestroyEntity(entity);

            Assert.IsFalse(entity.IsValid());
        }

        [Test]
        public void EntityStripAllExcept()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();
            entity.AddComponent(new TestComponent1());
            entity.AddComponent(new TestComponent2());
            entity.AddComponent(new TestComponent3());

            entity.StripAllComponentsExcept(typeof(TestComponent2));

            Assert.IsTrue(entity.HasComponent<TestComponent2>());

            Assert.IsFalse(entity.HasComponent<TestComponent1>());
            Assert.IsFalse(entity.HasComponent<TestComponent3>());
        }
    }
}
