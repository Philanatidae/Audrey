using System;
using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// Represents a family of entities.
    /// </summary>
    /// <remarks>
    /// A family is essentially a filter for components within entities.
    /// </remarks>
    public class Family : IEquatable<Family>
    {
        readonly Type[] _allComponents;
        readonly Type[] _oneComponents;
        readonly Type[] _noneComponents;

        internal Family(Type[] all, Type[] one, Type[] none)
        {
            _allComponents = all;
            _oneComponents = one;
            _noneComponents = none;
        }

        /// <summary>
        /// Determines if an entity matches the family
        /// </summary>
        /// <returns><c>true</c>, if <paramref name="entity"/> matches the samily,
        /// <c>false</c> otherwise.</returns>
        /// <param name="entity">Entity to check.</param>
        public bool Matches(Entity entity)
        {
            // Entity must have all of these components
            for (int i = 0; i < _allComponents.Length; i++)
            {
                Type comp = _allComponents[i];

                if (!entity.HasComponent(comp))
                {
                    return false;
                }
            }

            // Entity must have at least one of these components
            if (_oneComponents.Length > 0)
            {
                bool hasOne = false;
                for (int i = 0; i < _oneComponents.Length; i++)
                {
                    Type comp = _oneComponents[i];

                    if (entity.HasComponent(comp))
                    {
                        hasOne = true;
                        break;
                    }
                }

                if (!hasOne)
                {
                    return false;
                }
            }

            // Entity must have none of these components
            for (int i = 0; i < _noneComponents.Length; i++)
            {
                Type comp = _noneComponents[i];

                if (entity.HasComponent(comp))
                {
                    return false;
                }
            }

            // Passed the filter; matched the family
            return true;
        }

        public bool Equals(Family other)
        {
            return _allComponents.IsEquivalent(other._allComponents)
                                 && _oneComponents.IsEquivalent(other._oneComponents)
                                 && _noneComponents.IsEquivalent(other._noneComponents);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            Family other = obj as Family;
            if (other == null)
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

                for (int i = 0; i < _allComponents.Length; i++)
                {
                    hash = hash * 31 + _allComponents[i].GetHashCode();
                }

                for (int i = 0; i < _oneComponents.Length; i++)
                {
                    hash = hash * 31 + _oneComponents[i].GetHashCode();
                }

                for (int i = 0; i < _noneComponents.Length; i++)
                {
                    hash = hash * 31 + _noneComponents[i].GetHashCode();
                }

                return hash;
            }
        }

        public override string ToString()
        {
            string[] allComps = new string[_allComponents.Length];
            for (int i = 0; i < _allComponents.Length; i++)
            {
                allComps[i] = _allComponents[i].ToString();
            }

            string[] oneComps = new string[_oneComponents.Length];
            for (int i = 0; i < _oneComponents.Length; i++)
            {
                oneComps[i] = _oneComponents[i].ToString();
            }

            string[] noneComps = new string[_noneComponents.Length];
            for (int i = 0; i < _noneComponents.Length; i++)
            {
                noneComps[i] = _noneComponents[i].ToString();
            }

            return string.Format("[Family]\nAll: {0}\nOne: {1}\nNone: {2}",
                                 string.Join(", ", allComps),
                                 string.Join(", ", oneComps),
                                 string.Join(", ", noneComps));
        }

        /// <summary>
        /// Entities must contain all of the following components.
        /// </summary>
        /// <remarks>
        /// Constructs a new FamilyBuilder to build a Family.
        /// </remarks>
        /// <returns>FamilyBuilder for building a family.</returns>
        /// <param name="components">Components for which entities must have all of.</param>
        public static FamilyBuilder All(params Type[] components)
        {
            return new FamilyBuilder().All(components);
        }

        /// <summary>
        /// Entities must have one of the following components.
        /// </summary>
        /// <remarks>
        /// Constructs a new FamilyBuilder to build a Family.
        /// </remarks>
        /// <returns>FamilyBuilder for building a family.</returns>
        /// <param name="components">Components for which entities must have at least one of.</param>
        public static FamilyBuilder One(params Type[] components)
        {
            return new FamilyBuilder().One(components);
        }

        /// <summary>
        /// Entities must have none of the following components.
        /// </summary>
        /// <remarks>
        /// Constructs a new FamilyBuilder to build a Family.
        /// </remarks>
        /// <returns>FamilyBuilder for building a family.</returns>
        /// <param name="components">Components for which entities must have none of.</param>
        public static FamilyBuilder Exclude(params Type[] components)
        {
            return new FamilyBuilder().Exclude(components);
        }
    }

    /// <summary>
    /// A helper class for building a Family.
    /// </summary>
    public class FamilyBuilder
    {
        internal FamilyBuilder()
        {
        }

        List<Type> _allComponents = new List<Type>();
        List<Type> _oneComponents = new List<Type>();
        List<Type> _noneComponents = new List<Type>();

        /// <summary>
        /// Entities must have all of the following components.
        /// </summary>
        /// <returns>FamilyBuilder for building a Family.</returns>
        /// <param name="components">Components for which entities must have all of.</param>
        public FamilyBuilder All(params Type[] components)
        {
            _allComponents.AddRange(components);
            return this;
        }

        /// <summary>
        /// Entities must have one of the following components.
        /// </summary>
        /// <returns>FamilyBuilder for building a Family.</returns>
        /// <param name="components">Components for which entities must have at least one of.</param>
        public FamilyBuilder One(params Type[] components)
        {
            _oneComponents.AddRange(components);
            return this;
        }

        /// <summary>
        /// Entities must have none of the following components.
        /// </summary>
        /// <returns>FamilyBuilder for building a Family.</returns>
        /// <param name="components">Components for which entities must have none of.</param>
        public FamilyBuilder Exclude(params Type[] components)
        {
            _noneComponents.AddRange(components);
            return this;
        }

        /// <summary>
        /// Builds and retrieves the built Family.
        /// </summary>
        /// <returns>Built Family.</returns>
        public Family Get()
        {
            return new Family(_allComponents.ToArray(),
                              _oneComponents.ToArray(),
                              _noneComponents.ToArray());
        }
    }
}
