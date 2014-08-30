using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    /// <summary>
    /// Struct StateProbability
    /// </summary>
    [DebuggerDisplay("{State}, P={Probability}")]
    class StateProbability
    {
        /// <summary>
        /// The state
        /// </summary>
        [NotNull]
        public IState State { get; private set; }

        /// <summary>
        /// The probability
        /// </summary>
        public double Probability { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateProbability" /> struct.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="probability">The probability.</param>
        /// <exception cref="System.ArgumentNullException">state
        /// or
        /// probability</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public StateProbability([NotNull] IState state, double probability)
        {
            if (state == null) throw new ArgumentNullException("state");
            if (probability < 0 || probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "The probability value must be in range 0..1");
            if (Double.IsNaN(probability) || Double.IsInfinity(probability)) throw new NotFiniteNumberException("The value must be a finite number.", probability);

            State = state;
            Probability = probability;
        }
    }
}
