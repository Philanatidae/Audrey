using Audrey.Exceptions;
using System;
using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// Engine manages Entities.
    /// </summary>
    public class Engine
    {
        readonly internal EntityMap _entityMap;

        internal Dictionary<Type, IComponentMap> _components = new Dictionary<Type, IComponentMap>();

        internal Dictionary<Family, FamilyManager> _familyManagers = new Dictionary<Family, FamilyManager>();

        /// <summary>
        /// Constructs an entity Engine.
        /// </summary>
        public Engine()
        {
            _entityMap = new EntityMap(this);
        }

        /// <summary>
        /// Creates an Entity.
        /// </summary>
        /// <returns>Entity created within the Engine.</returns>
        public Entity CreateEntity()
        {
            Entity entity = _entityMap.CreateEntity();

            foreach(IComponentMap componentMap in _components.Values)
            {
                componentMap.AddEmptyEntity();
            }
            foreach(FamilyManager familyMap in _familyManagers.Values)
            {
                familyMap.AddEmptyEntity();
            }

            return entity;
        }

        /// <summary>
        /// Destroys an Entity. When an Entity is destroyed, it
        /// is no longer valid within the Engine an no longer
        /// belongs in any families. The component instances are
        /// transferred to the Entity itself, becoming independent.
        /// 
        /// Independent Entities can have components added/removed,
        /// and matches against a Family in case there are any hard
        /// references to the Entity itself. Otherwise, the garbage
        /// collector will clean up the Entity and its components
        /// (this is the normal use case).
        /// </summary>
        /// <param name="entity">Entity to destroy.</param>
        public void DestroyEntity(Entity entity)
        {
            if(!_entityMap.IsEntityValid(entity.EntityID))
            {
                return;
            }

            _entityMap.RemoveEntity(entity.EntityID);
        }
        /// <summary>
        /// Destroys all entities in the Engine.
        /// </summary>
        public void DestroyAllEntities()
        {
            while(GetEntities().Count > 0)
            {
                DestroyEntity(GetEntities()[0]);
            }
        }
        /// <summary>
        /// Destroys all entities matching a Family.
        /// </summary>
        /// <param name="family">Family to match against.</param>
        public void DestroyEntitiesFor(Family family)
        {
            while(GetEntitiesFor(family).Count > 0)
            {
                DestroyEntity(GetEntitiesFor(family)[0]);
            }
        }

        internal T AddComponent<T>(int entityID, T comp) where T : class, IComponent
        {
            return (T)AddRawComponent(entityID, comp);
        }
        internal IComponent AddRawComponent(int entityID, IComponent component)
        {
            if (!_entityMap.IsEntityValid(entityID))
            {
                throw new EntityNotValidException();
            }

            Type type = component.GetType();

            if (!_components.ContainsKey(type))
            {
                Type componentMapType = typeof(ComponentMap<>).MakeGenericType(type);
                _components.Add(type, (IComponentMap)Activator.CreateInstance(componentMapType));
                _components[type].Initialize(this);
            }

            return _components[type].AddRawComponent(entityID, component);
        }

        internal void RemoveComponent<T>(int entityID) where T : class, IComponent
        {
            RemoveComponent(entityID, typeof(T));
        }
        internal void RemoveComponent(int entityID, Type type)
        {
            if (!_entityMap.IsEntityValid(entityID))
            {
                throw new EntityNotValidException();
            }

            if(!_components.ContainsKey(type))
            {
                return;
            }

            _components[type].RemoveComponent(entityID);
        }

        internal T GetComponent<T>(int entityID) where T : class, IComponent
        {
            return (T)GetComponent(entityID, typeof(T));
        }
        internal IComponent GetComponent(int entityID, Type compType)
        {
            if(!typeof(IComponent).IsAssignableFrom(compType))
            {
                throw new TypeNotComponentException();
            }

            if (!_entityMap.IsEntityValid(entityID))
            {
                throw new EntityNotValidException();
            }

            if (!_components.ContainsKey(compType))
            {
                return null;
            }

            return _components[compType].GetComponent(entityID);
        }

        internal bool HasComponent<T>(int entityID) where T : class, IComponent
        {
            return HasComponent(entityID, typeof(T));
        }
        internal bool HasComponent(int entityID, Type type)
        {
            if (!_entityMap.IsEntityValid(entityID))
            {
                throw new EntityNotValidException();
            }

            if (!_components.ContainsKey(type))
            {
                return false;
            }

            return _components[type].GetComponent(entityID) != null;
        }

        /// <summary>
        /// Retrieves an ImmutableList<Entity> of all the
        /// entities in the Engine. This list is automatically
        /// updated since its contents are stored by reference.
        /// </summary>
        /// <returns>ImmutableList<Entity> of the entities in the Engine.</returns>
        public ImmutableList<Entity> GetEntities()
        {
            return _entityMap.Entities;
        }
        /// <summary>
        /// Retrieves an ImmutableList<Entity> of all the
        /// entities in the Engine matching a Family. This list is
        /// automatically updated since its contacts are stored by
        /// reference.
        /// </summary>
        /// <param name="family">Family to match against.</param>
        /// <returns>ImmutableList<Entity> of the entities in the Engine.</returns>
        public ImmutableList<Entity> GetEntitiesFor(Family family)
        {
            if(!_familyManagers.ContainsKey(family))
            {
                FamilyManager familyMap = new FamilyManager(family, this);
                familyMap.Initialize();
                _familyManagers.Add(family, familyMap);

                foreach(Type compType in family._allComponents)
                {
                    if(!_components.ContainsKey(compType))
                    {
                        Type componentMapType = typeof(ComponentMap<>).MakeGenericType(compType);
                        _components.Add(compType, (IComponentMap)Activator.CreateInstance(componentMapType));
                        _components[compType].Initialize(this);
                    }

                    _components[compType].RegisterFamilyManager(familyMap);
                }
                foreach (Type compType in family._oneComponents)
                {
                    if (!_components.ContainsKey(compType))
                    {
                        Type componentMapType = typeof(ComponentMap<>).MakeGenericType(compType);
                        _components.Add(compType, (IComponentMap)Activator.CreateInstance(componentMapType));
                        _components[compType].Initialize(this);
                    }

                    _components[compType].RegisterFamilyManager(familyMap);
                }
                foreach (Type compType in family._excludeComponents)
                {
                    if (!_components.ContainsKey(compType))
                    {
                        Type componentMapType = typeof(ComponentMap<>).MakeGenericType(compType);
                        _components.Add(compType, (IComponentMap)Activator.CreateInstance(componentMapType));
                        _components[compType].Initialize(this);
                    }

                    _components[compType].RegisterFamilyManager(familyMap);
                }
            }
            return _familyManagers[family].FamilyEntities;
        }
    }
}
