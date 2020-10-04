using Audrey.Tests.Components;
using NUnit.Framework;

namespace Audrey.Tests
{
    [TestFixture]
    public class FamilyTests
    {
        private void CreateTestEntities(Engine engine)
        {
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new TestComponent1());
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new TestComponent1());
                entity.AddComponent(new TestComponent2());
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new TestComponent1());
                entity.AddComponent(new TestComponent3());
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new TestComponent2());
                entity.AddComponent(new TestComponent3());
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AddComponent(new TestComponent1());
                entity.AddComponent(new TestComponent2());
                entity.AddComponent(new TestComponent3());
                entity.AddComponent(new TestComponent4());
            }
        }

        [Test]
        public void FamilyAll()
        {
            Engine engine = new Engine();
            CreateTestEntities(engine);

            Assert.IsTrue(engine.GetEntitiesFor(Family.All(typeof(TestComponent1)).Get()).Count == 4);
            Assert.IsTrue(engine.GetEntitiesFor(Family.All(typeof(TestComponent1), typeof(TestComponent2)).Get()).Count == 2);
        }
        [Test]
        public void FamilyOne()
        {
            Engine engine = new Engine();
            CreateTestEntities(engine);

            Assert.IsTrue(engine.GetEntitiesFor(Family.One(typeof(TestComponent1)).Get()).Count == 4);
            Assert.IsTrue(engine.GetEntitiesFor(Family.One(typeof(TestComponent1), typeof(TestComponent2)).Get()).Count == 5);
            Assert.IsTrue(engine.GetEntitiesFor(Family.One(typeof(TestComponent2), typeof(TestComponent4)).Get()).Count == 3);
        }
        [Test]
        public void FamilyExclude()
        {
            Engine engine = new Engine();
            CreateTestEntities(engine);

            Assert.IsTrue(engine.GetEntitiesFor(Family.Exclude(typeof(TestComponent1)).Get()).Count == 1);
            Assert.IsTrue(engine.GetEntitiesFor(Family.Exclude(typeof(TestComponent2)).Get()).Count == 2);
            Assert.IsTrue(engine.GetEntitiesFor(Family.All(typeof(TestComponent2)).Exclude(typeof(TestComponent4)).Get()).Count == 2);
        }

        [Test]
        public void FamilyAddToAll()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();
            entity.AddComponent(new TestComponent1());

            ImmutableList<Entity> entities = engine.GetEntitiesFor(Family.All(typeof(TestComponent2)).Get());
            Assert.IsTrue(entities.Count == 0);

            entity.AddComponent(new TestComponent2());
            Assert.IsTrue(entities.Count == 1);

            entity.RemoveComponent<TestComponent1>();
            Assert.IsTrue(entities.Count == 1);

            entity.RemoveComponent<TestComponent2>();
            Assert.IsTrue(entities.Count == 0);
        }

        [Test]
        public void FamilyAddToOne()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();

            ImmutableList<Entity> entities = engine.GetEntitiesFor(Family.One(typeof(TestComponent1), typeof(TestComponent2)).Get());
            Assert.IsTrue(entities.Count == 0);

            entity.AddComponent(new TestComponent1());
            Assert.IsTrue(entities.Count == 1);

            entity.AddComponent(new TestComponent2());
            Assert.IsTrue(entities.Count == 1);

            entity.RemoveComponent<TestComponent1>();
            Assert.IsTrue(entities.Count == 1);

            entity.RemoveComponent<TestComponent2>();
            Assert.IsTrue(entities.Count == 0);
        }

        [Test]
        public void FamilyAddToExclude()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();
            entity.AddComponent(new TestComponent1());

            ImmutableList<Entity> entities = engine.GetEntitiesFor(Family.All(typeof(TestComponent1)).Exclude(typeof(TestComponent4)).Get());
            Assert.IsTrue(entities.Count == 1);

            entity.AddComponent(new TestComponent4());
            Assert.IsTrue(entities.Count == 0);

            entity.RemoveComponent<TestComponent1>();
            Assert.IsTrue(entities.Count == 0);

            entity.RemoveComponent<TestComponent4>();
            Assert.IsTrue(entities.Count == 0);

            entity.AddComponent(new TestComponent1());
            Assert.IsTrue(entities.Count == 1);
        }
    }
}
