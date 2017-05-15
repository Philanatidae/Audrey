using System;
using System.Collections.Generic;
using Audrey.Exceptions;

namespace Audrey
{
    /// <summary>
    /// A collection of IComponent.
    /// </summary>
    public class Entity
    {
        readonly Engine _engine;
        List<IComponent> _components = new List<IComponent>();

        internal Entity(Engine engine)
        {
            _engine = engine;
            Family.All(typeof(Family)).Get();
        }

        /// <summary>
        /// Determines if the Entity has a IComponent.
        /// </summary>
        /// <returns><c>true</c>, if the Entity has the IComponent, <c>false</c> otherwise.</returns>
        /// <typeparam name="T">IComponent to check.</typeparam>
        public bool HasComponent<T>() where T : IComponent
        {
            return HasComponent(typeof(T));
        }

        /// <summary>
        /// Determines if the Entity has a IComponent.
        /// </summary>
        /// <returns><c>true</c>, if the Entity has the IComponent, <c>false</c> otherwise.</returns>
        /// <param name="componentType">IComponent type to check.</param>
        public bool HasComponent(Type componentType)
        {
            if (!componentType.IsComponent())
            {
                throw new TypeNotComponentException();
            }

            return GetComponent(componentType) != null;
        }

        /// <summary>
        /// Retrieves an IComponent instance for this Entity.
        /// </summary>
        /// <returns>IComponent instance if found, null otherwise.</returns>
        /// <typeparam name="T">IComponent to retrieve.</typeparam>
        public T GetComponent<T>() where T : IComponent
        {
            return (T)GetComponent(typeof(T));
        }

        /// <summary>
        /// Retrieves a component from the Entity.
        /// </summary>
        /// <returns>IComponent instance if found, null otherwise.</returns>
        /// <param name="componentType">IComponent type to retrieve.</param>
        public object GetComponent(Type componentType)
        {
            if (!componentType.IsComponent())
            {
                throw new TypeNotComponentException();
            }

            IComponent foundComp = _components.Find((IComponent comp) =>
            {
                return comp.GetType() == componentType;
            });

            return foundComp;
        }

        /// <summary>
        /// Add a component to this Entity.
        /// </summary>
        /// <param name="component">Component to add.</param>
        public void AddComponent(IComponent component)
        {
            if (HasComponent(component.GetType()))
            {
                throw new ComponentAlreadyExistsException();
            }

            _components.Add(component);
            // Update caches
            _engine.UpdateFamilyBags(this);
        }

        /// <summary>
        /// Removes a component from the Entity.
        /// </summary>
        /// <typeparam name="T">IComponent to remove.</typeparam>
        public void RemoveComponent<T>() where T : IComponent
        {
            RemoveComponent(typeof(T));
        }

        /// <summary>
        /// Removes a component from the Entity.
        /// </summary>
        /// <param name="componentType">IComponent type to remove.</param>
        public void RemoveComponent(Type componentType)
        {
            if (!componentType.IsComponent())
            {
                throw new TypeNotComponentException();
            }

            if (!HasComponent(componentType))
            {
                throw new ComponentNotFoundException();
            }

            IComponent componentToRemove = (IComponent)GetComponent(componentType);

            _components.Remove(componentToRemove);
            // Update caches
            _engine.UpdateFamilyBags(this);
        }
    }
}