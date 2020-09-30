using System;
using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// Manages an ImmutableList of Entities matching
    /// the Family.
    /// </summary>
    public class FamilyManager
    {
        /// <summary>
        /// Engine this FamilyManager belongs to.
        /// </summary>
        public Engine Engine
        {
            get;
            private set;
        }

        /// <summary>
        /// Family of this FamilyManager.
        /// </summary>
        public Family Family
        {
            get;
            private set;
        }

        private List<int> _familyEntityIndices = new List<int>();
        private List<Entity> _familyEntities = new List<Entity>();
        /// <summary>
        /// Entities belonging to the Family.
        /// </summary>
        public ImmutableList<Entity> FamilyEntities { get; private set; }

        /// <summary>
        /// Contains a map per Component type in the
        /// `All` filter flagging if an Entity has the
        /// associated component.
        /// </summary>
        private Dictionary<Type, List<bool>> _allComponentsMap = new Dictionary<Type, List<bool>>();
        /// <summary>
        /// Contains a map per Component type in the
        /// `One` filter flagging if an Entity has the
        /// associated component.
        /// </summary>
        private Dictionary<Type, List<bool>> _oneComponentsMap = new Dictionary<Type, List<bool>>();
        /// <summary>
        /// Contains a map per Component type in the
        /// `Exclude` filter flagging if an Entity has the
        /// associated component.
        /// </summary>
        private Dictionary<Type, List<bool>> _excludeComponentsMap = new Dictionary<Type, List<bool>>();

        /// <summary>
        /// Constructs a FamilyManager from a Family.
        /// </summary>
        /// <param name="family">Family of the FamilyManager.</param>
        public FamilyManager(Family family, Engine engine)
        {
            Engine = engine;
            Family = family;

            FamilyEntities = new ImmutableList<Entity>(_familyEntities);

            BuildAllComponentsMap();
            BuildOneComponentsMap();
            BuildExcludeComponentsMap();
        }

        /// <summary>
        /// Populates `_allComponentsMap` from the Family.
        /// </summary>
        private void BuildAllComponentsMap()
        {
            foreach (Type type in Family._allComponents)
            {
                _allComponentsMap.Add(type, new List<bool>());
            }
        }
        /// <summary>
        /// Populates `_oneComponentsMap` from the Family.
        /// </summary>
        private void BuildOneComponentsMap()
        {
            foreach (Type type in Family._oneComponents)
            {
                _oneComponentsMap.Add(type, new List<bool>());
            }
        }
        /// <summary>
        /// Populates `_excludeComponentsMap` from the Family.
        /// </summary>
        private void BuildExcludeComponentsMap()
        {
            foreach (Type type in Family._excludeComponents)
            {
                _excludeComponentsMap.Add(type, new List<bool>());
            }
        }

        /// <summary>
        /// Initializes the FamilyManager.
        /// </summary>
        public void Initialize()
        {
            // Loop over every entity, valid or not
            for (int i = 0; i < Engine.Entities.Count; i++)
            {
                // Loop through `All` Components
                foreach (Type compType in _allComponentsMap.Keys)
                {
                    // Add a placeholder of `false`
                    _allComponentsMap[compType].Add(false);
                    // If valid
                    if (Engine.Entities[i] != null)
                    {
                        // If the entity has the component
                        if (Engine.HasComponent(i, compType))
                        {
                            // Simulate the component being added to the entity within the FamilyManager
                            ComponentAdded(i, compType);
                        }
                    }
                }
                // Loop through `One` Components
                foreach (Type compType in _oneComponentsMap.Keys)
                {
                    // Add a placeholder of `false`
                    _oneComponentsMap[compType].Add(false);
                    // If valid
                    if (Engine.Entities[i] != null)
                    {
                        // If the entity has the component
                        if (Engine.HasComponent(i, compType))
                        {
                            // Simulate the component being added to the entity within the FamilyManager
                            ComponentAdded(i, compType);
                        }
                    }
                }
                // Loop through `Exclude` Components
                foreach (Type compType in _excludeComponentsMap.Keys)
                {
                    // Add a placeholder of `false`
                    _excludeComponentsMap[compType].Add(false);
                    // If valid
                    if (Engine.Entities[i] != null)
                    {
                        // If the entity has the component
                        if (Engine.HasComponent(i, compType))
                        {
                            // Simulate the component being added to the entity within the FamilyManager
                            ComponentAdded(i, compType);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Adds a new Entity to the FamilyManager sparse set.
        /// </summary>
        public void AddEmptyEntity()
        {
            // Add an additional placeholder at the end of the
            // sparse sets for each Family filter type.
            foreach (List<bool> comps in _allComponentsMap.Values)
            {
                comps.Add(false);
            }
            foreach (List<bool> comps in _oneComponentsMap.Values)
            {
                comps.Add(false);
            }
            foreach (List<bool> comps in _excludeComponentsMap.Values)
            {
                comps.Add(false);
            }
        }

        /// <summary>
        /// Called when a Component is added to an
        /// Entity via a ComponentMap.
        /// </summary>
        /// <typeparam name="T">Component added.</typeparam>
        /// <param name="entityID">ID of the Entity.</param>
        internal void ComponentAdded<T>(int entityID)
        {
            ComponentAdded(entityID, typeof(T));
        }
        /// <summary>
        /// Called when a Component is added to an
        /// Entity via a ComponentMap.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <param name="compTyp">Type of the Component.</param>
        internal void ComponentAdded(int entityID, Type compTyp)
        {
            if (_allComponentsMap.ContainsKey(compTyp))
            {
                _allComponentsMap[compTyp][entityID] = true;
            }
            if (_oneComponentsMap.ContainsKey(compTyp))
            {
                _oneComponentsMap[compTyp][entityID] = true;
            }
            if (_excludeComponentsMap.ContainsKey(compTyp))
            {
                _excludeComponentsMap[compTyp][entityID] = true;
            }

            // If a Component was added, and the entity
            // is not in the family yet, check to see if
            // it now qualifies.
            if (!IsEntityInFamilyList(entityID)
                && CheckIfEntityMatchesFamily(entityID))
            {
                AddEntityToFamily(entityID);
            } else
            {
                // If the entity qualifies and a Component was added,
                // the entity has the potential to not be qualified
                // if the component was an `Exclude` component.
                foreach (List<bool> comps in _excludeComponentsMap.Values)
                {
                    if (comps.Count > entityID
                        && comps[entityID])
                    {
                        RemoveEntityFromFamily(entityID);
                    }
                }
            }
        }
        /// <summary>
        /// Called when a Component is removed from
        /// an Entity via a ComponentMap.
        /// </summary>
        /// <typeparam name="T">Component removed.</typeparam>
        /// <param name="entityID">ID of the Entity.</param>
        internal void ComponentRemoved<T>(int entityID)
        {
            ComponentRemoved(entityID, typeof(T));
        }
        /// <summary>
        /// Called when a Component is removed from
        /// an Entity via a ComponentMap.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <param name="compType">Component removed.</param>
        internal void ComponentRemoved(int entityID, Type compType) // A, B, C
        {
            // Keep track if the entity was removed, we
            // don't need to go through removal checks
            // if the entity is no longer in the family.
            bool entityRemoved = false;

            if (_allComponentsMap.ContainsKey(compType))
            {
                _allComponentsMap[compType][entityID] = false;

                // Since an `All` component was removed, the
                // entity is guarenteed to no longer qualify.
                if (!entityRemoved && IsEntityInFamilyList(entityID))
                {
                    RemoveEntityFromFamily(entityID);
                    entityRemoved = true;
                }
            }
            if (_oneComponentsMap.ContainsKey(compType))
            {
                _oneComponentsMap[compType][entityID] = false;

                // A `One` component being removed means that they
                // have the potential (but not guaranteed) to not qualify
                // anymore.
                if (!entityRemoved && IsEntityInFamilyList(entityID))
                {
                    bool containsOne = false;
                    foreach (List<bool> comps in _oneComponentsMap.Values)
                    {
                        if (comps[entityID])
                        {
                            containsOne = true;
                            break;
                        }
                    }
                    if (!containsOne)
                    {
                        RemoveEntityFromFamily(entityID);
                        entityRemoved = true;
                    }
                }
            }
            if (_excludeComponentsMap.ContainsKey(compType))
            {
                _excludeComponentsMap[compType][entityID] = false;

                // A Component being removed has the potential to make a not
                // qualified entity a qualified one.
                if (!entityRemoved && !IsEntityInFamilyList(entityID)
                    && CheckIfEntityMatchesFamily(entityID))
                {
                    AddEntityToFamily(entityID);
                }
            }
        }

        /// <summary>
        /// Checks if an Entity matches the Familyl.
        /// </summary>
        /// <param name="entityID">ID of the Entity.</param>
        /// <returns>True if Entity matches the Family, false otherwise.</returns>
        private bool CheckIfEntityMatchesFamily(int entityID)
        {
            // Exclude components are usually short
            // and entity is guarenteed to not qualify if
            // it has any of them.
            foreach (List<bool> comps in _excludeComponentsMap.Values)
            {
                if (comps.Count > entityID
                    && comps[entityID])
                {
                    return false;
                }
            }
            // All components are simpler (on average) since they
            // have the potential to stop early if a component
            // is missing.
            foreach (List<bool> comps in _allComponentsMap.Values)
            {
                if (comps.Count - 1 < entityID
                    || !comps[entityID])
                {
                    return false;
                }
            }
            // Finally check `One` components
            if (_oneComponentsMap.Count > 0)
            {
                bool containsOne = false;
                foreach (List<bool> comps in _oneComponentsMap.Values)
                {
                    if (comps.Count - 1 < entityID)
                    {
                        continue;
                    }

                    if (comps[entityID])
                    {
                        containsOne = true;
                        break;
                    }
                }
                if (!containsOne)
                {
                    return false;
                }
            }

            // Fail = return, so if we reached this point
            // the entity qualifies.
            return true;
        }

        private void AddEntityToFamily(int entityID)
        {
            if (IsEntityInFamilyList(entityID))
            {
                return;
            }
            _familyEntityIndices.Add(entityID);
            _familyEntities.Add(Engine.Entities[entityID]);
        }
        private void RemoveEntityFromFamily(int entityID)
        {
            if(!IsEntityInFamilyList(entityID))
            {
                return;
            }
            int idx = _familyEntityIndices.IndexOf(entityID);
            _familyEntityIndices.RemoveAt(idx);
            _familyEntities.RemoveAt(idx);
        }
        private bool IsEntityInFamilyList(int entityID)
        {
            return _familyEntityIndices.Contains(entityID);
        }
    }
}
