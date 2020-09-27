using System;
using System.Collections.Generic;

namespace Audrey
{
    public class Engine
    {
        internal List<int> Entities
        {
            get;
            private set;
        } = new List<int>(); // Index = entity ID, value = version?
        Dictionary<Type, IComponentMap> _components = new Dictionary<Type, IComponentMap>();

        Dictionary<Family, FamilyMap> _familyMaps = new Dictionary<Family, FamilyMap>();

        public Engine()
        {
        }

        public int CreateEntity()
        {
            Entities.Add(Entities.Count);
            int entityID = Entities[Entities.Count - 1];

            foreach(IComponentMap componentMap in _components.Values)
            {
                componentMap.AddEmptyEntity();
            }
            foreach(FamilyMap familyMap in _familyMaps.Values)
            {
                familyMap.AddEmptyEntity();
            }

            return entityID;
        }

        public T AssignComponent<T>(int entityID) where T : class, IComponent, new()
        {
            if(entityID > Entities.Count - 1)
            {
                throw new Exception();
            }

            Type type = typeof(T);

            if(!_components.ContainsKey(type))
            {
                _components.Add(type, new ComponentMap<T>());
                _components[type].Initialize(Entities.Count);
            }

            return (T)((ComponentMap<T>)_components[type]).AssignComponent(entityID);
        }

        public void RemoveComponent<T>(int entityID) where T : class, IComponent, new()
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

        public T GetComponent<T>(int entityID) where T : class, IComponent, new()
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

        public bool HasComponent<T>(int entityID) where T : class, IComponent, new()
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

        public ImmutableList<int> GetEntitiesFor(Family family)
        {
            if(!_familyMaps.ContainsKey(family))
            {
                FamilyMap familyMap = new FamilyMap(family);
                familyMap.Initialize(this);
                _familyMaps.Add(family, familyMap);

                foreach(Type compType in family._allComponents)
                {
                    if(!_components.ContainsKey(compType))
                    {
                        Type componentMapType = typeof(ComponentMap<>).MakeGenericType(compType);
                        _components.Add(compType, (IComponentMap)Activator.CreateInstance(componentMapType));
                        _components[compType].Initialize(Entities.Count);
                    }

                    _components[compType].NotifyFamilyMap(familyMap);
                }
                foreach (Type compType in family._oneComponents)
                {
                    if (!_components.ContainsKey(compType))
                    {
                        Type componentMapType = typeof(ComponentMap<>).MakeGenericType(compType);
                        _components.Add(compType, (IComponentMap)Activator.CreateInstance(componentMapType));
                        _components[compType].Initialize(Entities.Count);
                    }

                    _components[compType].NotifyFamilyMap(familyMap);
                }
                foreach (Type compType in family._excludeComponents)
                {
                    if (!_components.ContainsKey(compType))
                    {
                        Type componentMapType = typeof(ComponentMap<>).MakeGenericType(compType);
                        _components.Add(compType, (IComponentMap)Activator.CreateInstance(componentMapType));
                        _components[compType].Initialize(Entities.Count);
                    }

                    _components[compType].NotifyFamilyMap(familyMap);
                }
            }
            return _familyMaps[family].Entities;
        }
    }
}
