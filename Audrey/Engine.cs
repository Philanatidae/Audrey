using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// A manager for Entities.
    /// </summary>
    public class Engine
    {
        readonly List<Entity> _entities = new List<Entity>();
        readonly ImmutableList<Entity> _immutableEntities;
        readonly Dictionary<Family, List<Entity>> _familyBags = new Dictionary<Family, List<Entity>>();
        readonly Dictionary<Family, ImmutableList<Entity>> _immutableFamilyBags = new Dictionary<Family, ImmutableList<Entity>>();

        /// <summary>
        /// Constructs a new Engine.
        /// </summary>
        public Engine()
        {
            // _immutableEntities will reference _entities
            _immutableEntities = new ImmutableList<Entity>(_entities);
        }

        /// <summary>
        /// Creates a new Entity within this Engine.
        /// </summary>
        /// <returns>Entity owned by this Engine.</returns>
        public Entity CreateEntity()
        {
            Entity entity = new Entity(this);
            _entities.Add(entity);

            // Don't need to update bags, entitiy does not have any components

            return entity;
        }

        /// <summary>
        /// Removes an Entity from this Engine.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void DestroyEntity(Entity entity)
        {
            _entities.Remove(entity);
            UpdateFamilyBags(entity);
        }

        /// <summary>
        /// Gets a ImmutableList<Entity> reference for the Entities in the Engine.
        /// </summary>
        /// <returns>Entities within this Engine.</returns>
        public ImmutableList<Entity> GetEntities()
        {
            // Return immutable list so it can not be changed
            return _immutableEntities;
        }

        /// <summary>
        /// Gets a ImmutableList<Entity> reference for the Entities in the Engine that matches the Family.
        /// </summary>
        /// <returns>Entities within this Engine that match the Family.</returns>
        /// <param name="family">Family to match Entities with.</param>
        public ImmutableList<Entity> GetEntitiesFor(Family family)
        {
            if (!_familyBags.ContainsKey(family))
            {
                InitFamilyBag(family);
            }

            // Return immutable list so it can not be changed
            return _immutableFamilyBags[family];
        }

        void InitFamilyBag(Family family)
        {
            List<Entity> bag = new List<Entity>();
            _familyBags.Add(family, bag);
            // Make an immutable list to reference the real list
            _immutableFamilyBags.Add(family, new ImmutableList<Entity>(_familyBags[family]));

            for (int i = 0; i < _entities.Count; i++)
            {
                Entity entity = _entities[i];

                if (family.Matches(entity))
                {
                    bag.Add(entity);
                }
            }
        }

        internal void UpdateFamilyBags(Entity entity)
        {
            foreach (Family family in _familyBags.Keys)
            {
                UpdateFamilyBag(family, entity);
            }
        }

        void UpdateFamilyBag(Family family, Entity entity)
        {
            List<Entity> bag = _familyBags[family];
            if (_entities.Contains(entity)) // Addition/update
            {
                // Entity matches the family but is not in the bag
                if (family.Matches(entity) && !bag.Contains(entity))
                {
                    bag.Add(entity);
                }
                // Entity does not match the family but is in the bag
                if (!family.Matches(entity) && bag.Contains(entity))
                {
                    bag.Remove(entity);
                }
            }
            else
            { // Removal
                bag.Remove(entity);
            }
        }
    }
}
