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
    }
}
