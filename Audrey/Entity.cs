using Audrey.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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

        internal Dictionary<Type, IComponent> _components = null;

        internal Entity(Engine engine, int entityID)
        {
            Engine = engine;
            EntityID = entityID;
        }

        /// <summary>
        /// Adds a component to the Entity.
        /// </summary>
        /// <typeparam name="T">Component type to add.</typeparam>
        /// <param name="comp">Component to add.</param>
        /// <returns>An instance of the component.</returns>
        public T AddComponent<T>(T comp) where T : class, IComponent
        {
            if(IsIndependent())
            {
                if(_components.ContainsKey(typeof(T)))
                {
                    throw new ComponentAlreadyExistsException();
                }

                _components.Add(typeof(T), comp);
                return comp;
            }

            return Engine.AddComponent<T>(EntityID, comp);
        }
        /// <summary>
        /// Adds a component to the Entity. Not preferred over
        /// `AssignComponent<T>`, however is sometimes useful.
        /// </summary>
        /// <param name="component">Component to add to the Entity.</param>
        /// <returns>An instance of the component.</returns>
        public IComponent AddRawComponent(IComponent component)
        {
            if(IsIndependent())
            {
                if (_components.ContainsKey(component.GetType()))
                {
                    throw new ComponentAlreadyExistsException();
                }

                _components.Add(component.GetType(), component);
                return component;
            }

            return Engine.AddRawComponent(EntityID, component);
        }
        /// <summary>
        /// Removes a component from the Entity.
        /// </summary>
        /// <typeparam name="T">Component to remove.</typeparam>
        public void RemoveComponent<T>() where T : class, IComponent
        {
            if(IsIndependent())
            {
                if(_components.ContainsKey(typeof(T)))
                {
                    _components.Remove(typeof(T));
                }
                return;
            }

            Engine.RemoveComponent<T>(EntityID);
        }
        /// <summary>
        /// Removes a component from the Entity. Not preferred over
        /// `RemoveComponent<T>`, however is sometimes useful.
        /// </summary>
        /// <param name="componentType">Component to remove.</param>
        public void RemoveRawComponent(Type componentType)
        {
            if (IsIndependent())
            {
                if (!typeof(IComponent).IsAssignableFrom(componentType))
                {
                    throw new TypeNotComponentException();
                }

                if (_components.ContainsKey(componentType))
                {
                    _components.Remove(componentType);
                }
                return;
            }

            Engine.RemoveComponent(EntityID, componentType);
        }
        /// <summary>
        /// Retrieves a component from the Entity.
        /// </summary>
        /// <typeparam name="T">Component to retrieve.</typeparam>
        /// <returns>An instance of the component, or null if the entity does not have the component.</returns>
        public T GetComponent<T>() where T : class, IComponent
        {
            if(IsIndependent())
            {
                if(_components.ContainsKey(typeof(T)))
                {
                    return (T)_components[typeof(T)];
                }
                return null;
            }

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
            if (IsIndependent())
            {
                if (_components.ContainsKey(componentType))
                {
                    return _components[componentType];
                }
                return null;
            }

            return Engine.GetComponent(EntityID, componentType);
        }
        /// <summary>
        /// Checks if the Entity has the Component.
        /// </summary>
        /// <typeparam name="T">Component to check against.</typeparam>
        /// <returns>True if the Entity contains the component, false otherwise.</returns>
        public bool HasComponent<T>() where T : class, IComponent
        {
            if(IsIndependent())
            {
                return GetComponent<T>() != null;
            }

            return Engine.HasComponent<T>(EntityID);
        }

        /// <summary>
        /// Returns an array of component instances for
        /// this Entity.
        /// </summary>
        /// <returns>Array of components.</returns>
        public IComponent[] GetComponents()
        {
            if(IsIndependent())
            {
                return _components.Values.ToArray();
            }

            return Engine._entityMap.GetComponents(EntityID);
        }

        /// <summary>
        /// Removes all components except for the component
        /// types specified.
        /// </summary>
        /// <param name="types">Component types to not remove.</param>
        public void StripAllComponentsExcept(params Type[] types)
        {
            IComponent[] components = GetComponents();
            foreach(IComponent component in components)
            {
                if(!types.Contains(component.GetType()))
                {
                    RemoveRawComponent(component.GetType());
                }
            }
        }

        /// <summary>
        /// Checks if the Entity is valid within its Engine.
        /// </summary>
        /// <returns>True if the Entity is valid, false otherwise.</returns>
        public bool IsValid()
        {
            if(EntityID < 0)
            {
                return false;
            }

            return Engine._entityMap.IsEntityValid(EntityID);
        }

        internal bool IsIndependent()
        {
            return EntityID == -1;
        }
        internal void ConvertToIndependentEntity()
        {
            _components = new Dictionary<Type, IComponent>();

            foreach (IComponent component in GetComponents())
            {
                Type componentType = component.GetType();

                if (GetRawComponent(componentType) != null)
                {
                    RemoveRawComponent(componentType);

                    _components.Add(componentType, component);
                }
            }

            EntityID = -1;
            Engine = null;
        }
    }
}
