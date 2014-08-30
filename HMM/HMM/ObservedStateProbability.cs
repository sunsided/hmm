using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    /// <summary>
    /// Struct StateProbability
    /// </summary>
    [DebuggerDisplay("P({State}|{Observation})={Probability}")]
    sealed class ObservedStateProbability : StateProbability
    {
        /// <summary>
        /// Gets the observation.
        /// </summary>
        /// <value>The observation.</value>
        [NotNull]
        public IObservation Observation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateProbability" /> struct.
        /// </summary>
        /// <param name="observation">The observation.</param>
        /// <param name="state">The state.</param>
        /// <param name="probability">The probability.</param>
        /// <exception cref="System.ArgumentNullException">state
        /// or
        /// observation</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">state
        /// or
        /// observation</exception>
        /// <exception cref="System.NotFiniteNumberException">probability;The probability value must be in range 0..1</exception>
        public ObservedStateProbability([NotNull] IObservation observation, [NotNull] IState state, double probability) : base(state, probability)
        {
            if (observation == null) throw new ArgumentNullException("observation");

            Observation = observation;
        }
    }
}
