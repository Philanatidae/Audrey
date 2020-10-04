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
            int entityID;
            if(_unusedEntityIDs.Count > 0)
            {
                entityID = _unusedEntityIDs[0];
                _unusedEntityIDs.RemoveAt(0);
            } else
            {
                entityID = _entityIndices.Count;
            }

            int idx = _entityList.Count;

            _entityIndices.Add(idx);

            _entityList.Add(entityID);
            _entityWrappers.Add(new Entity(Engine, entityID));

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

            _entityList.RemoveAt(idx);
            _entityWrappers.RemoveAt(idx);

            _entityIndices[entityID] = -1;

            _unusedEntityIDs.Add(entity.EntityID);
            entity.ConvertToIndependentEntity();
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

            return entityID < _entityIndices.Count;
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
    }
}
