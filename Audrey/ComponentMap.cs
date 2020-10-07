using Audrey.Exceptions;
using System;
using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// Interface of public functions from ComponentMap.
    /// </summary>
    public interface IComponentMap
    {
        void RegisterFamilyManager(FamilyManager familyMap);
        void Initialize(Engine engine);
        void AddEmptyEntity();
        IComponent AddRawComponent(int id, IComponent component);
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
    internal class ComponentMap<T> : IComponentMap where T : class, IComponent
    {
        private List<int> _entityIndices = new List<int>(); // Sparse set, index = entity ID, value = index of componnet

        private List<int> _entityList = new List<int>(); // Index = index of component, value = entity ID (reverse lookup)
        private List<T> _componentList = new List<T>();

        private List<FamilyManager> _familyManagers = new List<FamilyManager>(); // Family maps that this component type needs to update

        public ComponentMap()
        {
        }

        /// <summary>
        /// Initializes the component map with an Engine.
        /// </summary>
        /// <param name="engine">Engine this component map belongs to.</param>
        public void Initialize(Engine engine)
        {
            for (int i = 0; i < engine._entityMap.RawEntityCount; i++)
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
        /// <param name="familyManager">FamilyMap to notify of changes.</param>
        public void RegisterFamilyManager(FamilyManager familyManager)
        {
            if (!_familyManagers.Contains(familyManager))
            {
                _familyManagers.Add(familyManager);
            }
        }

        /// <summary>
        /// Adds a component to an entity.
        /// </summary>
        /// <param name="id">ID of the entity.</param>
        /// <returns>Instance of the component assigned to the entity.</returns>
        public IComponent AddRawComponent(int id, IComponent component)
        {
            if(!(component is T))
            {
                throw new ComponentDoesNotMatchException();
            }

            if(id > _entityIndices.Count - 1)
            {
                throw new EntityNotValidException();
            }

            int idx = _entityList.Count;
            _entityIndices[id] = idx;

            _entityList.Add(id);
            _componentList.Add((T)component);

            foreach(FamilyManager familyMap in _familyManagers)
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
                throw new EntityNotValidException();
            }

            int idx = _entityIndices[id];
            if(idx < 0)
            {
                return;
            }

            _entityList.RemoveAt(idx);
            _componentList.RemoveAt(idx);

            // Update _entityIndices to account for the _componentList becoming shorter
            for(int i = idx; i < _entityList.Count; i++)
            {
                _entityIndices[_entityList[i]]--;
            }

            _entityIndices[id] = -1;

            foreach (FamilyManager familyMap in _familyManagers)
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
                throw new EntityNotValidException();
            }

            if(_entityIndices[id] < 0)
            {
                return null;
            }

            return _componentList[_entityIndices[id]];
        }
    }
}
