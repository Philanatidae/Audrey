using System;
using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// Interface of public functions from ComponentMap.
    /// </summary>
    public interface IComponentMap
    {
        void NotifyFamilyMap(FamilyMap familyMap);
        void Initialize(Engine engine);
        void AddEmptyEntity();
        IComponent AssignComponent(int id);
        void RemoveComponent(int id);
        IComponent GetComponent(int id);
    }

    /// <summary>
    /// A map of a component type relating entity ID's to components. Components
    /// are owned inside the component map. Components are added/removed using this
    /// map.
    /// 
    /// When components are added/removed, families depending on the component type
    /// of the component map are notified of the change.
    /// </summary>
    /// <typeparam name="T">Component that this map will represent.</typeparam>
    internal class ComponentMap<T> : IComponentMap where T : class, IComponent, new()
    {
        private List<int> _entityIndices = new List<int>(); // Sparse set, index = entity ID, value = index of componnet

        private List<int> _entityList = new List<int>(); // Index = index of component, value = entity ID (reverse lookup)
        private List<T> _componentList = new List<T>();

        private List<FamilyMap> _familyMaps = new List<FamilyMap>(); // Family maps that this component type needs to update

        public ComponentMap()
        {
        }

        /// <summary>
        /// Initializes the component map with an Engine.
        /// </summary>
        /// <param name="engine">Engine this component map belongs to.</param>
        public void Initialize(Engine engine)
        {
            for (int i = 0; i < engine.Entities.Count; i++)
            {
                _entityIndices.Add(-1);
            }
        }
        /// <summary>
        /// Adds an entity to the sparse set in the component map.
        /// </summary>
        public void AddEmptyEntity()
        {
            _entityIndices.Add(-1);
        }

        /// <summary>
        /// Register a FamilyMap for the component map to notify of
        /// components being added/removed to/from an entity.
        /// </summary>
        /// <param name="familyMap">FamilyMap to notify of changes.</param>
        public void NotifyFamilyMap(FamilyMap familyMap)
        {
            if (!_familyMaps.Contains(familyMap))
            {
                _familyMaps.Add(familyMap);
            }
        }

        /// <summary>
        /// Assign a component to an entity.
        /// </summary>
        /// <param name="id">ID of the entity.</param>
        /// <returns>Instance of the component assigned to the entity.</returns>
        public IComponent AssignComponent(int id)
        {
            if(id > _entityIndices.Count - 1)
            {
                throw new Exception();
            }

            int idx = _entityList.Count;
            _entityIndices[id] = idx;

            _entityList.Add(id);
            _componentList.Add(new T());

            foreach(FamilyMap familyMap in _familyMaps)
            {
                familyMap.ComponentAdded<T>(id);
            }

            return _componentList[idx];
        }
        /// <summary>
        /// Removes a component from an entity.
        /// </summary>
        /// <param name="id">ID of the entity.</param>
        public void RemoveComponent(int id)
        {
            if (id > _entityIndices.Count - 1)
            {
                throw new Exception();
            }

            int idx = _entityIndices[id];
            if(idx < 0)
            {
                return;
            }

            _entityList.RemoveAt(idx);
            _componentList.RemoveAt(idx);

            _entityIndices[id] = -1;

            foreach (FamilyMap familyMap in _familyMaps)
            {
                familyMap.ComponentRemoved<T>(id);
            }
        }
        /// <summary>
        /// Retrieves a component from an entity if the entity
        /// owns a component, otherwise return null.
        /// </summary>
        /// <param name="id">ID of the entity.</param>
        /// <returns>Component assigned to the entity or null.</returns>
        public IComponent GetComponent(int id)
        {
            if (id > _entityIndices.Count - 1)
            {
                throw new Exception();
            }

            if(_entityIndices[id] < 0)
            {
                return null;
            }

            return _componentList[_entityIndices[id]];
        }
    }
}
