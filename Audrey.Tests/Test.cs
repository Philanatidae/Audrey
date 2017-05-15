using NUnit.Framework;
using System;
using Audrey.Exceptions;

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
        public void EntityComponents()
        {
            Engine engine = new Engine();
            Entity entity = engine.CreateEntity();

            {
                AudreyComponent1 comp = new AudreyComponent1();
                entity.AddComponent(comp);
                Assert.True(entity.HasComponent<AudreyComponent1>());

                AudreyComponent1 backComp = entity.GetComponent<AudreyComponent1>();
                Assert.NotNull(backComp);
                Assert.AreSame(comp, backComp);

                entity.RemoveComponent<AudreyComponent1>();
                Assert.False(entity.HasComponent<AudreyComponent1>());
                Assert.NotNull(comp);
            }

            Assert.Catch(typeof(ComponentNotFoundException),
                        entity.RemoveComponent<AudreyComponent1>);

            {
                entity.AddComponent(new AudreyComponent1());
                Assert.Catch(typeof(ComponentAlreadyExistsException), () =>
                {
                    entity.AddComponent(new AudreyComponent1());
                });
                entity.RemoveComponent<AudreyComponent1>();
            }

            Assert.DoesNotThrow(() =>
            {
                entity.GetComponent<AudreyComponent1>();
            });
            Assert.IsNull(entity.GetComponent<AudreyComponent1>());
        }

        [Test]
        public void Families()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();

            entity.AddComponent(new AudreyComponent1());
            Assert.True(Family.All(typeof(AudreyComponent1)).Get().Matches(entity));
            Assert.False(Family.All(typeof(AudreyComponent1), typeof(AudreyComponent2)).Get().Matches(entity));
            Assert.True(Family.All(typeof(AudreyComponent1)).Exclude(typeof(AudreyComponent2)).Get().Matches(entity));
            Assert.True(Family.All(typeof(AudreyComponent1)).Exclude(typeof(AudreyComponent2), typeof(AudreyComponent3)).Get().Matches(entity));
            Assert.True(Family.One(typeof(AudreyComponent1)).Get().Matches(entity));
            Assert.False(Family.Exclude(typeof(AudreyComponent1)).Get().Matches(entity));
            Assert.True(Family.Exclude(typeof(AudreyComponent2)).Get().Matches(entity));

            entity.AddComponent(new AudreyComponent2());
            Assert.True(Family.All(typeof(AudreyComponent1)).Get().Matches(entity));
            Assert.True(Family.All(typeof(AudreyComponent1), typeof(AudreyComponent2)).Get().Matches(entity));
            Assert.False(Family.All(typeof(AudreyComponent1), typeof(AudreyComponent2), typeof(AudreyComponent3)).Get().Matches(entity));
            Assert.True(Family.One(typeof(AudreyComponent1)).Get().Matches(entity));
            Assert.True(Family.One(typeof(AudreyComponent1), typeof(AudreyComponent2)).Get().Matches(entity));
            Assert.True(Family.One(typeof(AudreyComponent1), typeof(AudreyComponent2), typeof(AudreyComponent3)).Get().Matches(entity));
            Assert.False(Family.One(typeof(AudreyComponent1)).Exclude(typeof(AudreyComponent2)).Get().Matches(entity));

            {
                Family family1 = Family.All(typeof(AudreyComponent1)).One(typeof(AudreyComponent2)).Exclude(typeof(AudreyComponent3)).Get();
                Family family2 = Family.All(typeof(AudreyComponent1)).One(typeof(AudreyComponent2)).Exclude(typeof(AudreyComponent3)).Get();

                Assert.AreEqual(family1, family2);
            }
        }

        [Test]
        public void EngineQueries()
        {
            Engine engine = new Engine();

            Entity entity = engine.CreateEntity();

            ImmutableList<Entity> entities = engine.GetEntities();
            Assert.NotNull(entities);
            Assert.AreEqual(entities.Count, 1);

            {
                ImmutableList<Entity> entities2 = engine.GetEntities();
                Assert.AreSame(entities, entities2);
            }

            ImmutableList<Entity> audrey1Entities = engine.GetEntitiesFor(Family.All(typeof(AudreyComponent1)).Get());
            Assert.NotNull(audrey1Entities);
            Assert.AreEqual(audrey1Entities.Count, 0);

            {
                ImmutableList<Entity> Audrey1Entities2 = engine.GetEntitiesFor(Family.All(typeof(AudreyComponent1)).Get());
                Assert.AreSame(audrey1Entities, Audrey1Entities2);
            }

            entity.AddComponent(new AudreyComponent1());
            Assert.AreEqual(audrey1Entities.Count, 1);

            entity.RemoveComponent<AudreyComponent1>();
            Assert.AreEqual(audrey1Entities.Count, 0);
        }
    }

    public class AudreyComponent1 : IComponent { }
    public class AudreyComponent2 : IComponent { }
    public class AudreyComponent3 : IComponent { }
}
