using System;
using System.Collections.Generic;
using System.Text;

namespace Audrey
{
    public interface IComponentMap
    {
        void NotifyFamilyMap(FamilyMap familyMap);
        void Initialize(int entityCount);
        void AddEmptyEntity();
        IComponent AssignComponent(int id);
        void RemoveComponent(int id);
        IComponent GetComponent(int id);
    }

    public class ComponentMap<T> : IComponentMap where T : class, IComponent, new()
    {
        private List<int> _entityIndices = new List<int>();

        private List<int> _entityList = new List<int>();
        private List<T> _componentList = new List<T>();

        private List<FamilyMap> _familyMaps = new List<FamilyMap>();

        public ComponentMap()
        {
        }

        public void Initialize(int entityCount)
        {
            for (int i = 0; i < entityCount; i++)
            {
                _entityIndices.Add(-1);
            }
        }
        public void AddEmptyEntity()
        {
            _entityIndices.Add(-1);
        }

        public void NotifyFamilyMap(FamilyMap familyMap)
        {
            if (!_familyMaps.Contains(familyMap))
            {
                _familyMaps.Add(familyMap);
            }
        }

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
