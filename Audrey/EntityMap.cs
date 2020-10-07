using System;
using System.Collections.Generic;
using System.Text;

namespace Audrey
{
    /// <summary>
    /// Manages entity ID's and Entity wrapper.s
    /// </summary>
    public class EntityMap
    {
        /// <summary>
        /// Engine this EntityMap belongs to.
        /// </summary>
        public Engine Engine
        {
            get;
            private set;
        }

        private List<int> _entityIndices = new List<int>(); // Sparse
        private List<int> _entityList = new List<int>(); // Packed, points to _entityIndices
        private List<Entity> _entityWrappers = new List<Entity>(); // Packed, associated with _entityList
        private List<List<IComponent>> _entityComponents = new List<List<IComponent>>(); // Packed, associated with _entityList
        private List<List<FamilyManager>> _entityFamilies = new List<List<FamilyManager>>();

        private List<int> _unusedEntityIDs = new List<int>();

        /// <summary>
        /// Packed immutable list of Entity wrappers.
        /// </summary>
        public ImmutableList<Entity> Entities;

        /// <summary>
        /// Count of all the entities (valid and invalid) in the Engine.
        /// 
        /// This is the same as the current capacity of the Engine.
        /// </summary>
        public int RawEntityCount => _entityIndices.Count;

        /// <summary>
        /// Constructs a new EntityMap.
        /// </summary>
        /// <param name="engine">Engine this EntityMap belongs to.</param>
        public EntityMap(Engine engine)
        {
            Engine = engine;
            Entities = new ImmutableList<Entity>(_entityWrappers);
        }

        /// <summary>
        /// Creates an entity.
        /// </summary>
        /// <returns>Entity wrapper of the entity ID.</returns>
        public Entity CreateEntity()
        {
            int idx = _entityList.Count;

            int entityID;
            if(_unusedEntityIDs.Count > 0)
            {
                entityID = _unusedEntityIDs[0];
                _unusedEntityIDs.RemoveAt(0);
                _entityIndices[entityID] = idx;
            } else
            {
                entityID = _entityIndices.Count;
                _entityIndices.Add(idx);
            }

            _entityList.Add(entityID);
            _entityWrappers.Add(new Entity(Engine, entityID));
            _entityComponents.Add(new List<IComponent>());
            _entityFamilies.Add(new List<FamilyManager>());

            return _entityWrappers[_entityIndices[entityID]];
        }

        /// <summary>
        /// Removes an entity from an entity ID.
        /// </summary>
        /// <param name="entityID">ID of the entity.</param>
        public void RemoveEntity(int entityID)
        {
            int idx = _entityIndices[entityID];
            Entity entity = _entityWrappers[idx];

            FamilyManager[] familyManagers = _entityFamilies[idx].ToArray();
            foreach(FamilyManager familyManager in familyManagers)
            {
                familyManager.EntityDestroyed(entityID);
            }

            _unusedEntityIDs.Add(entity.EntityID);
            entity.ConvertToIndependentEntity();

            _entityList.RemoveAt(idx);
            _entityWrappers.RemoveAt(idx);
            _entityComponents.RemoveAt(idx);
            _entityFamilies.RemoveAt(idx);

            // Update _entityIndices to account for the _componentList becoming shorter
            for (int i = idx; i < _entityList.Count; i++)
            {
                _entityIndices[_entityList[i]]--;
            }
            
            _entityIndices[entityID] = -1;
        }

        /// <summary>
        /// Called when a component is added to an Entity.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <param name="component">Component instance to be added.</param>
        public void AddComponent(int entityID, IComponent component)
        {
            if(!IsEntityValid(entityID))
            {
                return;
            }

            _entityComponents[_entityIndices[entityID]].Add(component);
        }
        /// <summary>
        /// Called when a component is removed from an Entity.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <param name="component">Component instance to be removed.</param>
        public void RemoveComponent(int entityID, IComponent component)
        {
            if (!IsEntityValid(entityID))
            {
                return;
            }

            _entityComponents[_entityIndices[entityID]].Remove(component);
        }
        /// <summary>
        /// Returns an array of component instances for an Entity.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <returns>Array of components for the Entity.</returns>
        public IComponent[] GetComponents(int entityID)
        {
            if (!IsEntityValid(entityID))
            {
                return null;
            }

            return _entityComponents[_entityIndices[entityID]].ToArray();
        }

        /// <summary>
        /// Checks if the entity is valid within the Engine.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <returns>True if the entity is valid within the Engine, false otherwise.</returns>
        public bool IsEntityValid(int entityID)
        {
            if(entityID < 0)
            {
                return false;
            }
            if(entityID > _entityIndices.Count - 1)
            {
                return false;
            }
            return _entityIndices[entityID] > -1;
        }

        /// <summary>
        /// Retrieves the Entity wrapper from an entity ID.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <returns>Instance of Entity wrapper based on the ID, null if the entity is not valid.</returns>
        public Entity GetEntityWrapperFromID(int entityID)
        {
            if(!IsEntityValid(entityID))
            {
                return null;
            }

            return _entityWrappers[_entityIndices[entityID]];
        }

        public void AddEntityToFamily(int entityID, FamilyManager familyManager)
        {
            if (!IsEntityValid(entityID))
            {
                return;
            }

            _entityFamilies[_entityIndices[entityID]].Add(familyManager);
        }
        public void RemoveEntityFromFamily(int entityID, FamilyManager familyManager)
        {
            if (!IsEntityValid(entityID))
            {
                return;
            }

            _entityFamilies[_entityIndices[entityID]].Remove(familyManager);
        }
    }
}
