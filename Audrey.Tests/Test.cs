using NUnit.Framework;

namespace Audrey.Tests
{
    [TestFixture]
    public class Test
    {
        [Test]
        public void EntityCreation()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();
            Assert.NotNull(entity);
        }

        [Test]
        public void EntityGetComponent()
        {
            Engine engine = new Engine();
            Entity entity = engine.CreateEntity();

            AudreyComponent1 comp = entity.AssignComponent<AudreyComponent1>();
            Assert.IsNotNull(comp);

            AudreyComponent1 getComp = entity.GetComponent<AudreyComponent1>();
            Assert.AreEqual(comp, getComp);
        }

        [Test]
        public void Families()
        {
            Engine engine = new Engine();
            #region ENTITIES
            {
                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent1>();
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent1>();
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent1>();
                entity.AssignComponent<AudreyComponent2>();
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent1>();
                entity.AssignComponent<AudreyComponent2>();
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent2>();
            }
            {
                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent2>();
            }
            #endregion

            {
                Family family = Family.One(typeof(AudreyComponent1), typeof(AudreyComponent2)).Get();

                ImmutableList<Entity> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 6);
            }
            {
                Family family = Family.All(typeof(AudreyComponent1)).Get();

                ImmutableList<Entity> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 4);
            }

            {
                Family family = Family.All(typeof(AudreyComponent1), typeof(AudreyComponent3)).Get();

                ImmutableList<Entity> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 0);

                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent1>();
                entity.AssignComponent<AudreyComponent3>();
                Assert.IsTrue(entities.Count == 1);

                entity.RemoveComponent<AudreyComponent1>();
                Assert.IsTrue(entities.Count == 0);

                entity.AssignComponent<AudreyComponent1>();
            }

            {
                Family family = Family.All(typeof(AudreyComponent1)).Exclude(typeof(AudreyComponent2)).Get();

                ImmutableList<Entity> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 3);
            }

            {
                Family family = Family.All(typeof(AudreyComponent1)).Exclude(typeof(AudreyComponent3)).Get();

                Entity entity = engine.CreateEntity();
                entity.AssignComponent<AudreyComponent1>();

                ImmutableList<Entity> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 5);

                entity.AssignComponent<AudreyComponent3>();
                Assert.IsTrue(entities.Count == 4);

                entity.RemoveComponent<AudreyComponent3>();
                Assert.IsTrue(entities.Count == 5);
            }
        }

        [Test]
        public void EngineQueries()
        {
            //Engine engine = new Engine();

            //Entity entity = engine.CreateEntity();

            //ImmutableList<Entity> entities = engine.GetEntities();
            //Assert.NotNull(entities);
            //Assert.AreEqual(entities.Count, 1);

            //{
            //    ImmutableList<Entity> entities2 = engine.GetEntities();
            //    Assert.AreSame(entities, entities2);
            //}

            //ImmutableList<Entity> audrey1Entities = engine.GetEntitiesFor(Family.All(typeof(AudreyComponent1)).Get());
            //Assert.NotNull(audrey1Entities);
            //Assert.AreEqual(audrey1Entities.Count, 0);

            //{
            //    ImmutableList<Entity> Audrey1Entities2 = engine.GetEntitiesFor(Family.All(typeof(AudreyComponent1)).Get());
            //    Assert.AreSame(audrey1Entities, Audrey1Entities2);
            //}

            //entity.AddComponent(new AudreyComponent1());
            //Assert.AreEqual(audrey1Entities.Count, 1);

            //entity.RemoveComponent<AudreyComponent1>();
            //Assert.AreEqual(audrey1Entities.Count, 0);
        }
    }

    public class AudreyComponent1 : IComponent { }
    public class AudreyComponent2 : IComponent { }
    public class AudreyComponent3 : IComponent { }
}
