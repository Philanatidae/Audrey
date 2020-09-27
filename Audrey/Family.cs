using System;
using System.Collections.Generic;

namespace Audrey
{
    public class Family
    {
        internal readonly Type[] _allComponents;
        internal readonly Type[] _oneComponents;
        internal readonly Type[] _excludeComponents;

        private readonly string[] _allComponentsNames;
        private readonly string[] _oneComponentsNames;
        private readonly string[] _excludeComponentsNames;

        internal Family(Type[] allComponents, Type[] oneComponents, Type[] excludeComponents)
        {
            _allComponents = allComponents;
            _allComponentsNames = GenerateNames(_allComponents);
            _oneComponents = oneComponents;
            _oneComponentsNames = GenerateNames(_oneComponents);
            _excludeComponents = excludeComponents;
            _excludeComponentsNames = GenerateNames(_excludeComponents);
        }

        private string[] GenerateNames(Type[] types)
        {
            string[] names = new string[types.Length];
            for (int i = 0; i < types.Length; i++)
            {
                names[i] = types[i].ToString();
            }
            return names;
        }

        public static FamilyBuilder All(params Type[] types)
        {
            FamilyBuilder familyBuilder = new FamilyBuilder();
            return familyBuilder.All(types);
        }
        public static FamilyBuilder One(params Type[] types)
        {
            FamilyBuilder familyBuilder = new FamilyBuilder();
            return familyBuilder.One(types);
        }
        public static FamilyBuilder Exclude(params Type[] types)
        {
            FamilyBuilder familyBuilder = new FamilyBuilder();
            return familyBuilder.Exclude(types);
        }

        public bool Matches(Engine engine, int entityID)
        {
            foreach(Type type in _allComponents)
            {
                if(!engine.HasComponent(entityID, type))
                {
                    return false;
                }
            }
            bool containsOne = false;
            foreach (Type type in _oneComponents)
            {
                if (engine.HasComponent(entityID, type))
                {
                    containsOne = true;
                    break;
                }
            }
            if(!containsOne)
            {
                return false;
            }

            foreach(Type type in _excludeComponents)
            {
                if(engine.HasComponent(entityID, type))
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(Family other)
        {
            return _allComponents.IsEquivalent(other._allComponents);
        }

        public override bool Equals(object obj)
        {
            if(obj == this)
            {
                return true;
            }
            if(obj == null)
            {
                return false;
            }

            Family other = obj as Family;
            if(other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach(Type type in _allComponents)
                {
                    hash = hash * 31 + type.GetHashCode();
                }
                foreach (Type type in _oneComponents)
                {
                    hash = hash * 31 + type.GetHashCode();
                }
                foreach (Type type in _excludeComponents)
                {
                    hash = hash * 31 + type.GetHashCode();
                }

                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format($"[Family]\nAll: {string.Join(", ", _allComponentsNames)}\n" +
                $"One: {string.Join(", ", _oneComponentsNames)}\n" +
                $"Exclude: {string.Join(", ", _excludeComponentsNames)}");
        }
    }

    public class FamilyBuilder
    {
        List<Type> _allComponents = new List<Type>();
        List<Type> _oneComponents = new List<Type>();
        List<Type> _excludeComponents = new List<Type>();

        internal FamilyBuilder()
        {
        }

        public FamilyBuilder All(params Type[] types)
        {
            _allComponents.AddRange(types);
            return this;
        }
        public FamilyBuilder One(params Type[] types)
        {
            _oneComponents.AddRange(types);
            return this;
        }
        public FamilyBuilder Exclude(params Type[] types)
        {
            _excludeComponents.AddRange(types);
            return this;
        }

        public Family Get()
        {
            return new Family(_allComponents.ToArray(), _oneComponents.ToArray(), _excludeComponents.ToArray());
        }
    }
}
