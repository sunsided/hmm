using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NER.HMM
{
    /// <summary>
    /// Struct LabeledObservation
    /// </summary>
    [DebuggerDisplay("{Observation}/{State}")]
    struct LabeledObservation
    {
        /// <summary>
        /// The state
        /// </summary>
        [NotNull]
        public readonly IState State;

        /// <summary>
        /// The observation
        /// </summary>
        [NotNull]
        public readonly IObservation Observation;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabeledObservation"/> struct.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// state
        /// or
        /// observation
        /// </exception>
        public LabeledObservation([NotNull] IState state, [NotNull] IObservation observation)
        {
            if (state == null) throw new ArgumentNullException("state");
            if (observation == null) throw new ArgumentNullException("observation");
            State = state;
            Observation = observation;
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="LabeledObservation" /> is equal to this instance.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns><see langword="true" /> if the specified <see cref="LabeledObservation" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(LabeledObservation other)
        {
            return State.Equals(other.State) && Observation.Equals(other.Observation);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">Another object to compare to.</param>
        /// <returns><see langword="true" /> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals([CanBeNull] object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is LabeledObservation && Equals((LabeledObservation) obj);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (State.GetHashCode()*397) ^ Observation.GetHashCode();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return String.Format("{0}/{1}", Observation, State);
        }
    }
}
