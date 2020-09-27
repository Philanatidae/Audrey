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

            int entityID = engine.CreateEntity();
            Assert.IsTrue(entityID == 0);
        }

        [Test]
        public void EntityGetComponent()
        {
            Engine engine = new Engine();
            int entityID = engine.CreateEntity();

            AudreyComponent1 comp = engine.AssignComponent<AudreyComponent1>(entityID);
            Assert.IsNotNull(comp);

            AudreyComponent1 getComp = engine.GetComponent<AudreyComponent1>(entityID);
            Assert.AreEqual(comp, getComp);
        }

        [Test]
        public void Families()
        {
            Engine engine = new Engine();
            #region ENTITIES
            {
                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent1>(entityID);
            }
            {
                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent1>(entityID);
            }
            {
                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent1>(entityID);
                engine.AssignComponent<AudreyComponent2>(entityID);
            }
            {
                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent1>(entityID);
                engine.AssignComponent<AudreyComponent2>(entityID);
            }
            {
                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent2>(entityID);
            }
            {
                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent2>(entityID);
            }
            #endregion

            {
                Family family = Family.One(typeof(AudreyComponent1), typeof(AudreyComponent2)).Get();

                ImmutableList<int> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 6);
            }
            {
                Family family = Family.All(typeof(AudreyComponent1)).Get();

                ImmutableList<int> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 4);
            }

            {
                Family family = Family.All(typeof(AudreyComponent1), typeof(AudreyComponent3)).Get();

                ImmutableList<int> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 0);

                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent1>(entityID);
                engine.AssignComponent<AudreyComponent3>(entityID);
                Assert.IsTrue(entities.Count == 1);

                engine.RemoveComponent<AudreyComponent1>(entityID);
                Assert.IsTrue(entities.Count == 0);

                engine.AssignComponent<AudreyComponent1>(entityID);
            }

            {
                Family family = Family.All(typeof(AudreyComponent1)).Exclude(typeof(AudreyComponent2)).Get();

                ImmutableList<int> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 3);
            }

            {
                Family family = Family.All(typeof(AudreyComponent1)).Exclude(typeof(AudreyComponent3)).Get();

                int entityID = engine.CreateEntity();
                engine.AssignComponent<AudreyComponent1>(entityID);

                ImmutableList<int> entities = engine.GetEntitiesFor(family);
                Assert.IsTrue(entities.Count == 5);

                engine.AssignComponent<AudreyComponent3>(entityID);
                Assert.IsTrue(entities.Count == 4);

                engine.RemoveComponent<AudreyComponent3>(entityID);
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
