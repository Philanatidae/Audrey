using System;
using System.Collections.Generic;

namespace Audrey
{
    public class Engine
    {
        internal List<Entity> Entities
        {
            get;
            private set;
        } = new List<Entity>();
        Dictionary<Type, IComponentMap> _components = new Dictionary<Type, IComponentMap>();

        Dictionary<Family, FamilyManager> _familyManagers = new Dictionary<Family, FamilyManager>();

        public Engine()
        {
        }

        public Entity CreateEntity()
        {
            Entities.Add(new Entity(this, Entities.Count));
            Entity entity = Entities[Entities.Count - 1];

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

        internal T AssignComponent<T>(int entityID) where T : class, IComponent, new()
        {
            if(entityID > Entities.Count - 1)
            {
                throw new Exception();
            }

            Type type = typeof(T);

            if(!_components.ContainsKey(type))
            {
                _components.Add(type, new ComponentMap<T>());
                _components[type].Initialize(this);
            }

            return (T)((ComponentMap<T>)_components[type]).AssignComponent(entityID);
        }

        internal void RemoveComponent<T>(int entityID) where T : class, IComponent, new()
        {
            RemoveComponent(entityID, typeof(T));
        }
        internal void RemoveComponent(int entityID, Type type)
        {
            if (entityID > Entities.Count - 1)
            {
                throw new Exception();
            }

            if(!_components.ContainsKey(type))
            {
                return;
            }

            _components[type].RemoveComponent(entityID);
        }

        internal T GetComponent<T>(int entityID) where T : class, IComponent, new()
        {
            if (entityID > Entities.Count - 1)
            {
                throw new Exception();
            }

            Type type = typeof(T);

            if (!_components.ContainsKey(type))
            {
                return null;
            }

            return (T)((ComponentMap<T>)_components[type]).GetComponent(entityID);
        }

        internal bool HasComponent<T>(int entityID) where T : class, IComponent, new()
        {
            return HasComponent(entityID, typeof(T));
        }
        internal bool HasComponent(int entityID, Type type)
        {
            if (entityID > Entities.Count - 1)
            {
                throw new Exception();
            }

            if (!_components.ContainsKey(type))
            {
                return false;
            }

            return _components[type].GetComponent(entityID) != null;
        }

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
