using System;
using System.Collections.Generic;
using System.Text;

namespace Audrey
{
    public class EntityMap
    {
        public Engine Engine
        {
            get;
            private set;
        }

        private List<int> _entityIndices = new List<int>(); // Sparse
        private List<int> _entityList = new List<int>(); // Packed, points to _entityIndices
        private List<Entity> _entityWrappers = new List<Entity>(); // Packed, associated with _entityList

        private List<int> _unusedEntityIDs = new List<int>();

        public ImmutableList<Entity> Entities;

        public int RawEntityCount => _entityIndices.Count;

        public EntityMap(Engine engine)
        {
            Engine = engine;
            Entities = new ImmutableList<Entity>(_entityWrappers);
        }

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

        public void RemoveEntity(int entityID)
        {
            int idx = _entityIndices[entityID];
            Entity entity = _entityWrappers[idx];

            _entityList.RemoveAt(idx);
            _entityWrappers.RemoveAt(idx);

            _entityIndices[entityID] = -1;

            _unusedEntityIDs.Add(entity.EntityID);
            entity.EntityID = -1;
        }

        public bool IsEntityValid(int entityID)
        {
            if(entityID < 0)
            {
                return false;
            }

            return entityID < _entityIndices.Count;
        }

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
