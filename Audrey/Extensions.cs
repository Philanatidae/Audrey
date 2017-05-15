using System;

namespace Audrey
{
    /// <summary>
    /// Defines several utility extensions used by Audrey.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines if a Type is an IComponent.
        /// </summary>
        /// <returns><c>true</c>, if <paramref name="type"/> is an IComponent, <c>false</c> otherwise.</returns>
        /// <param name="type">Type to check.</param>
        public static bool IsComponent(this Type type)
        {
            return typeof(IComponent).IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines if two arrays are equivalent.
        /// </summary>
        /// <remarks>
        /// Equivalency is used to describe two arrays that contain the same values,
        /// ignoring order.
        /// </remarks>
        /// <returns><c>true</c>, if the arrays are equivalent, <c>false</c> otherwise.</returns>
        /// <param name="a">The first array.</param>
        /// <param name="b">The second array.</param>
        public static bool IsEquivalent(this object[] a, object[] b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                object objA = a[i];

                bool found = false;
                for (int j = 0; j < b.Length; j++)
                {
                    object objB = b[i];

                    if (objA.Equals(objB))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
