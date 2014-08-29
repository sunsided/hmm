using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace NER
{
    /// <summary>
    /// Class LinqExtensions.
    /// </summary>
    internal static class LinqExtensions
    {
        /// <summary>
        /// Struct Pair
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [DebuggerDisplay("{Left}, {Right}")]
        public struct Pair<T>
        {
            /// <summary>
            /// The left item
            /// </summary>
            public readonly T Left;

            /// <summary>
            /// The right item
            /// </summary>
            public readonly T Right;

            /// <summary>
            /// Initializes a new instance of the <see cref="Pair{T}"/> struct.
            /// </summary>
            /// <param name="left">The left.</param>
            /// <param name="right">The right.</param>
            public Pair(T left, T right)
            {
                Left = left;
                Right = right;
            }
        }

        /// <summary>
        /// Enumerates the sequence pairwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns>IEnumerable&lt;Pair&lt;T&gt;&gt;.</returns>
        [NotNull]
        public static IEnumerable<Pair<T>> Pairwise<T>([NotNull] this IList<T> sequence)
        {
            return sequence.Zip(sequence.Skip(1), (t, u) => new Pair<T>(t, u));
        }

        /// <summary>
        /// Enumerates the sequence pairwise.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence">The sequence.</param>
        /// <returns>IEnumerable&lt;Pair&lt;T&gt;&gt;.</returns>
        [NotNull]
        public static IEnumerable<Pair<T>> Pairwise<T>([NotNull] this IEnumerable<T> sequence)
        {
            var prev = default(T);
            using (var e = sequence.GetEnumerator())
            {
                if (e.MoveNext()) prev = e.Current;
                while (e.MoveNext()) yield return new Pair<T>(prev, e.Current);
            }
        }
    }
}