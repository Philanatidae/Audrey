using System.Collections;
using System.Collections.Generic;

namespace Audrey
{
    /// <summary>
    /// Wrapper for a List<T> reference to create a list that cannot be changed.
    /// </summary>
    public class ImmutableList<T> : IEnumerable<T> where T : class
    {
        readonly List<T> _items;

        /// <summary>
        /// The count of items in the list.
        /// </summary>
        /// <value>Count of items.</value>
        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        /// <summary>
        /// Constructs a new ImmutableList<T> using a List<T> as a reference.
        /// </summary>
        /// <param name="items">List to use as a reference.</param>
        public ImmutableList(List<T> items)
        {
            _items = items;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        public T this[int key]
        {
            get
            {
                return _items[key];
            }
        }
    }
}
