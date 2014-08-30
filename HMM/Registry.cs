using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
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
        /// The read only collection
        /// </summary>
        private readonly Lazy<ReadOnlyCollection<T>> _readOnlyCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="Registry{T}"/> class.
        /// </summary>
        public Registry()
        {
            _readOnlyCollection = new Lazy<ReadOnlyCollection<T>>(() => _entries.AsReadOnly());
        }

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
            return _readOnlyCollection.Value;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Registry{"/> to <see cref="ReadOnlyCollection{`0}"/>.
        /// </summary>
        /// <param name="registry">The registry.</param>
        /// <returns>The result of the conversion.</returns>
        [NotNull]
        public static implicit operator ReadOnlyCollection<T>(Registry<T> registry)
        {
            return registry.AsReadOnly();
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
