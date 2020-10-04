using System;
using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// Specifies component make-up of entities belonging
    /// to a Family.
    /// </summary>
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

        /// <summary>
        /// Specifies components that entities must contain all of.
        /// </summary>
        /// <param name="types">Types of components that entities must contain all of.</param>
        /// <returns>FamilyBuilder</returns>
        public static FamilyBuilder All(params Type[] types)
        {
            FamilyBuilder familyBuilder = new FamilyBuilder();
            return familyBuilder.All(types);
        }
        /// <summary>
        /// Specifies components that entities must contain at least one of.
        /// </summary>
        /// <param name="types">Types of components that entities must contain all of.</param>
        /// <returns>FamilyBuilder</returns>
        public static FamilyBuilder One(params Type[] types)
        {
            FamilyBuilder familyBuilder = new FamilyBuilder();
            return familyBuilder.One(types);
        }
        /// <summary>
        /// Specifies components that entities must contain none of.
        /// </summary>
        /// <param name="types">Types of components that entities must contain none of.</param>
        /// <returns>FamilyBuilder</returns>
        public static FamilyBuilder Exclude(params Type[] types)
        {
            FamilyBuilder familyBuilder = new FamilyBuilder();
            return familyBuilder.Exclude(types);
        }

        /// <summary>
        /// Determines if an entity matches the component
        /// make-up of this Family.
        /// </summary>
        /// <param name="entity">Entity to compare against.</param>
        /// <returns>True if the entity matches the Family, otherwise false.</returns>
        public bool Matches(Entity entity)
        {
            foreach(Type type in _allComponents)
            {
                if(entity.GetRawComponent(type) == null)
                {
                    return false;
                }
            }
            bool containsOne = false;
            foreach (Type type in _oneComponents)
            {
                if (entity.GetRawComponent(type) != null)
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
                if(entity.GetRawComponent(type) != null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a Family is equivalent to this Family.
        /// </summary>
        /// <param name="other">Family to compare against.</param>
        /// <returns>True if equivalent, false otherwise.</returns>
        public bool Equals(Family other)
        {
            return _allComponents.IsEquivalent(other._allComponents);
        }

        /// <summary>
        /// Determines if an object is equivalent to this Family.
        /// </summary>
        /// <param name="other">Family to compare against.</param>
        /// <returns>True if equivalent, false otherwise.</returns>
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

        /// <summary>
        /// Specifies components that entities must contain all of.
        /// </summary>
        /// <param name="types">Types of components that entities must contain all of.</param>
        /// <returns>FamilyBuilder</returns>
        public FamilyBuilder All(params Type[] types)
        {
            _allComponents.AddRange(types);
            return this;
        }
        /// <summary>
        /// Specifies components that entities must contain at least one of.
        /// </summary>
        /// <param name="types">Types of components that entities must contain all of.</param>
        /// <returns>FamilyBuilder</returns>
        public FamilyBuilder One(params Type[] types)
        {
            _oneComponents.AddRange(types);
            return this;
        }
        /// <summary>
        /// Specifies components that entities must contain none of.
        /// </summary>
        /// <param name="types">Types of components that entities must contain none of.</param>
        /// <returns>FamilyBuilder</returns>
        public FamilyBuilder Exclude(params Type[] types)
        {
            _excludeComponents.AddRange(types);
            return this;
        }

        /// <summary>
        /// Returns a constructed Family object.
        /// </summary>
        /// <returns>Constructed Family object.</returns>
        public Family Get()
        {
            return new Family(_allComponents.ToArray(), _oneComponents.ToArray(), _excludeComponents.ToArray());
        }
    }
}
