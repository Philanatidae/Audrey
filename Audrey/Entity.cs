using Audrey.Exceptions;
using System;
using System.ComponentModel;

namespace Audrey
{
    /// <summary>
    /// An Entity within the Engine.
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// Engine this Entity belongs to.
        /// </summary>
        public Engine Engine
        {
            get;
            private set;
        }
        /// <summary>
        /// ID of the Entity within the Engine.
        /// </summary>
        internal int EntityID
        {
            get;
            set;
        }

        internal Entity(Engine engine, int entityID)
        {
            Engine = engine;
            EntityID = entityID;
        }

        /// <summary>
        /// Assigns an empty component to the Entity.
        /// </summary>
        /// <typeparam name="T">Component to assign.</typeparam>
        /// <returns>An instance of the component.</returns>
        public T AssignComponent<T>() where T : class, IComponent, new()
        {
            return Engine.AssignComponent<T>(EntityID);
        }
        /// <summary>
        /// Adds a component to the Entity. Not preferred over
        /// `AssignComponent<T>`, however is sometimes useful.
        /// </summary>
        /// <param name="component">Component to add to the Entity.</param>
        /// <returns>An instance of the component.</returns>
        public IComponent AddRawComponent(IComponent component)
        {
            return Engine.AddRawComponent(EntityID, component);
        }
        /// <summary>
        /// Removes a component from the Entity.
        /// </summary>
        /// <typeparam name="T">Component to remove.</typeparam>
        public void RemoveComponent<T>() where T : class, IComponent, new()
        {
            Engine.RemoveComponent<T>(EntityID);
        }
        /// <summary>
        /// Removes a component from the Entity. Not preferred over
        /// `RemoveComponent<T>`, however is sometimes useful.
        /// </summary>
        /// <param name="componentType">Component to remove.</param>
        public void RemoveRawComponent(Type componentType)
        {
            Engine.RemoveComponent(EntityID, componentType);
        }
        /// <summary>
        /// Retrieves a component from the Entity.
        /// </summary>
        /// <typeparam name="T">Component to retrieve.</typeparam>
        /// <returns>An instance of the component, or null if the entity does not have the component.</returns>
        public T GetComponent<T>() where T : class, IComponent, new()
        {
            return Engine.GetComponent<T>(EntityID);
        }
        /// <summary>
        /// Retrieves a component from the Entity. Not
        /// preferred over `GetComponent<T>`, however is sometimes useful.
        /// </summary>
        /// <param name="componentType">Component to retrieve.</param>
        /// <returns>An instance of the component, or null if the entity does not have the component.</returns>
        public IComponent GetRawComponent(Type componentType)
        {
            if (!typeof(IComponent).IsAssignableFrom(componentType))
            {
                throw new TypeNotComponentException();
            }
            return Engine.GetComponent(EntityID, componentType);
        }
        /// <summary>
        /// Checks if the Entity has the Component.
        /// </summary>
        /// <typeparam name="T">Component to check against.</typeparam>
        /// <returns>True if the Entity contains the component, false otherwise.</returns>
        public bool HasComponent<T>() where T : class, IComponent, new()
        {
            return Engine.HasComponent<T>(EntityID);
        }
    }
}
