using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    /// <summary>
    /// Class NamedState. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("State {Name}")]
    sealed class NamedState : IState
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [NotNull]
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedState"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public NamedState([NotNull] string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Determines whether the specified <see cref="NamedState" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="T:NamedState" /> to compare with the current <see cref="T:NamedState" />.</param>
        /// <returns><see langword="true" /> if the specified <see cref="NamedState" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        private bool Equals([NotNull] NamedState other)
        {
            return string.Equals(Name, other.Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is NamedState && Equals((NamedState) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
