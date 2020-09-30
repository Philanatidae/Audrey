namespace Audrey
{
    public class Entity
    {
        public Engine Engine
        {
            get;
            private set;
        }
        internal int EntityID
        {
            get;
            private set;
        }

        internal Entity(Engine engine, int entityID)
        {
            Engine = engine;
            EntityID = entityID;
        }

        public T AssignComponent<T>() where T : class, IComponent, new()
        {
            return Engine.AssignComponent<T>(EntityID);
        }
        public void RemoveComponent<T>() where T : class, IComponent, new()
        {
            Engine.RemoveComponent<T>(EntityID);
        }
        public T GetComponent<T>() where T : class, IComponent, new()
        {
            return Engine.GetComponent<T>(EntityID);
        }
        public bool HasComponent<T>() where T : class, IComponent, new()
        {
            return Engine.HasComponent<T>(EntityID);
        }
    }
}
