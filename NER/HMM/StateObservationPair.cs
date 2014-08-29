using System;
using JetBrains.Annotations;

namespace NER.HMM
{
    /// <summary>
    /// Struct StateObservationPair
    /// </summary>
    struct StateObservationPair
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
        /// Initializes a new instance of the <see cref="StateObservationPair"/> struct.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <exception cref="System.ArgumentNullException">
        /// state
        /// or
        /// observation
        /// </exception>
        public StateObservationPair([NotNull] IState state, [NotNull] IObservation observation)
        {
            if (state == null) throw new ArgumentNullException("state");
            if (observation == null) throw new ArgumentNullException("observation");
            State = state;
            Observation = observation;
        }
        
        /// <summary>
        /// Determines whether the specified <see cref="StateObservationPair" /> is equal to this instance.
        /// </summary>
        /// <param name="other">Another object to compare to.</param>
        /// <returns><see langword="true" /> if the specified <see cref="StateObservationPair" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(StateObservationPair other)
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
            return obj is StateObservationPair && Equals((StateObservationPair) obj);
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
    }
}
