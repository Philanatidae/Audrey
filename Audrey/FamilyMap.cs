using System;
using System.Collections.Generic;

namespace Audrey
{
    public class FamilyMap
    {
        public Family Family
        {
            get;
            private set;
        }

        private List<int> _entities = new List<int>();
        public ImmutableList<int> Entities { get; private set; }

        private Dictionary<Type, List<bool>> _allComponents = new Dictionary<Type, List<bool>>();
        private Dictionary<Type, List<bool>> _oneComponents = new Dictionary<Type, List<bool>>();
        private Dictionary<Type, List<bool>> _excludeComponents = new Dictionary<Type, List<bool>>();

        public FamilyMap(Family family)
        {
            Family = family;

            Entities = new ImmutableList<int>(_entities);

            BuildAllComponentsMap();
            BuildOneComponentsMap();
            BuildExcludeComponentsMap();
        }

        private void BuildAllComponentsMap()
        {
            foreach(Type type in Family._allComponents)
            {
                _allComponents.Add(type, new List<bool>());
            }
        }
        private void BuildOneComponentsMap()
        {
            foreach (Type type in Family._oneComponents)
            {
                _oneComponents.Add(type, new List<bool>());
            }
        }
        private void BuildExcludeComponentsMap()
        {
            foreach (Type type in Family._excludeComponents)
            {
                _excludeComponents.Add(type, new List<bool>());
            }
        }

        public void Initialize(Engine engine)
        {
            for (int i = 0; i < engine.Entities.Count; i++)
            {
                foreach(Type compType in _allComponents.Keys)
                {
                    _allComponents[compType].Add(false);
                    if (engine.Entities[i] > -1)
                    {
                        if (engine.HasComponent(i, compType))
                        {
                            ComponentAdded(i, compType);
                        }
                    }
                }
                foreach (Type compType in _oneComponents.Keys)
                {
                    _oneComponents[compType].Add(false);
                    if (engine.Entities[i] > -1)
                    {
                        if (engine.HasComponent(i, compType))
                        {
                            ComponentAdded(i, compType);
                        }
                    }
                }
                foreach (Type compType in _excludeComponents.Keys)
                {
                    _excludeComponents[compType].Add(false);
                    if (engine.Entities[i] > -1)
                    {
                        if (engine.HasComponent(i, compType))
                        {
                            ComponentAdded(i, compType);
                        }
                    }
                }
            }
        }
        public void AddEmptyEntity()
        {
            foreach(List<bool> comps in _allComponents.Values)
            {
                comps.Add(false);
            }
            foreach (List<bool> comps in _oneComponents.Values)
            {
                comps.Add(false);
            }
            foreach (List<bool> comps in _excludeComponents.Values)
            {
                comps.Add(false);
            }
        }

        internal void ComponentAdded<T>(int entityID)
        {
            ComponentAdded(entityID, typeof(T));
        }
        internal void ComponentAdded(int entityID, Type compTyp)
        {
            if (_allComponents.ContainsKey(compTyp)
                && _allComponents[compTyp].Count > entityID)
            {
                _allComponents[compTyp][entityID] = true;
            }
            if (_oneComponents.ContainsKey(compTyp)
                && _oneComponents[compTyp].Count > entityID)
            {
                _oneComponents[compTyp][entityID] = true;
            }
            if (_excludeComponents.ContainsKey(compTyp)
                && _excludeComponents[compTyp].Count > entityID)
            {
                _excludeComponents[compTyp][entityID] = true;
            }

            if (!_entities.Contains(entityID))
            {
                foreach(List<bool> comps in _excludeComponents.Values)
                {
                    if (comps.Count > entityID
                        && comps[entityID])
                    {
                        return;
                    }
                }
                foreach (List<bool> comps in _allComponents.Values)
                {
                    if (comps.Count - 1 < entityID
                        || !comps[entityID])
                    {
                        return;
                    }
                }
                if (_oneComponents.Count > 0)
                {
                    bool containsOne = false;
                    foreach (List<bool> comps in _oneComponents.Values)
                    {
                        if(comps.Count - 1 < entityID)
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
                        return;
                    }
                }
                _entities.Add(entityID);
            } else
            {
                foreach (List<bool> comps in _excludeComponents.Values)
                {
                    if (comps.Count > entityID
                        && comps[entityID])
                    {
                        _entities.Remove(entityID);
                    }
                }
            }
        }
        internal void ComponentRemoved<T>(int entityID)
        {
            ComponentRemoved(entityID, typeof(T));
        }
        internal void ComponentRemoved(int entityID, Type compType)
        {
            bool entityRemoved = false;

            if (_allComponents.ContainsKey(compType))
            {
                _allComponents[compType][entityID] = false;

                if (!entityRemoved && _entities.Contains(entityID))
                {
                    _entities.Remove(entityID);
                    entityRemoved = true;
                }
            }
            if (_oneComponents.ContainsKey(compType))
            {
                _oneComponents[compType][entityID] = false;

                if (!entityRemoved && _entities.Contains(entityID))
                {
                    bool containsOne = false;
                    foreach (List<bool> comps in _oneComponents.Values)
                    {
                        if (comps[entityID])
                        {
                            containsOne = true;
                            break;
                        }
                    }
                    if (!containsOne)
                    {
                        _entities.Remove(entityID);
                        entityRemoved = true;
                    }
                }
            }
            if (_excludeComponents.ContainsKey(compType))
            {
                _excludeComponents[compType][entityID] = false;

                if(!entityRemoved && !_entities.Contains(entityID))
                {
                    foreach (List<bool> comps in _excludeComponents.Values)
                    {
                        if (comps.Count > entityID
                            && comps[entityID])
                        {
                            return;
                        }
                    }
                    _entities.Add(entityID);
                }
            }
        }
    }
}
