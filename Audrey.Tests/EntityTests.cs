using Audrey.Tests.Components;
using NUnit.Framework;
using System.Linq;

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

        [Test]
        public void EntityGetComponents()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();

            TestComponent1 comp1 = new TestComponent1();
            TestComponent2 comp2 = new TestComponent2();

            entity.AddRawComponent(comp1);
            entity.AddRawComponent(comp2);

            Assert.IsTrue(entity.GetComponents().Contains(comp1));
            Assert.IsTrue(entity.GetComponents().Contains(comp2));
        }

        [Test]
        public void EntityIndependent()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();
            entity.AddComponent(new TestComponent1());
            entity.AddComponent(new TestComponent2());
            entity.AddComponent(new TestComponent3());
            entity.AddComponent(new TestComponent4());

            Assert.IsTrue(entity.IsValid());
            Assert.IsTrue(engine.GetEntitiesFor(Family.All(typeof(TestComponent1), typeof(TestComponent2)).Get()).Count == 1);

            engine.DestroyEntity(entity);

            Assert.IsFalse(entity.IsValid());

            Assert.IsTrue(entity.HasComponent<TestComponent1>());
            Assert.IsTrue(entity.HasComponent<TestComponent2>());
            Assert.IsTrue(entity.HasComponent<TestComponent3>());
            Assert.IsTrue(entity.HasComponent<TestComponent4>());

            Assert.IsTrue(Family.All(typeof(TestComponent1), typeof(TestComponent2)).Get().Matches(entity));
            Assert.IsFalse(Family.All(typeof(TestComponent1), typeof(TestComponent2)).Exclude(typeof(TestComponent4)).Get().Matches(entity));

            Assert.IsTrue(engine.GetEntitiesFor(Family.All(typeof(TestComponent1), typeof(TestComponent2)).Get()).Count == 0);
        }

        [Test]
        public void EntityDestroyAll()
        {
            Engine engine = new Engine();

            for (int i = 0; i < 10; i++)
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new TestComponent1());
            }

            Assert.IsTrue(engine.GetEntities().Count == 10);
            engine.DestroyAllEntities();
            Assert.IsTrue(engine.GetEntities().Count == 0);
        }

        [Test]
        public void EntityDestroy()
        {
            Engine engine = new Engine();

            for (int i = 0; i < 10; i++)
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new TestComponent1());
            }

            Assert.IsTrue(engine.GetEntities().Count == 10);
            engine.DestroyEntity(engine.GetEntities()[4]);
            Assert.IsTrue(engine.GetEntities().Count == 9);
            engine.DestroyEntity(engine.GetEntities()[2]);
            Assert.IsTrue(engine.GetEntities().Count == 8);
        }
    }
}
