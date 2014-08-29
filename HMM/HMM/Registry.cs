using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace NER.HMM
{
    /// <summary>
    /// Class Registry. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    sealed class Registry<T> : IEnumerable<T>
        where T: class
    {
        /// <summary>
        /// The entries
        /// </summary>
        private readonly List<T> _entries = new List<T>();

        /// <summary>
        /// Adds the specified entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns>T.</returns>
        [NotNull]
        public T Add([NotNull] T entry)
        {
            _entries.Add(entry);
            return entry;
        }

        /// <summary>
        /// Ases the read only.
        /// </summary>
        /// <returns>ReadOnlyCollection&lt;T&gt;.</returns>
        [NotNull]
        public ReadOnlyCollection<T> AsReadOnly()
        {
            return _entries.AsReadOnly();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
