using System.Diagnostics;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    /// <summary>
    /// Struct ChainedStateProbability
    /// </summary>
    [DebuggerDisplay("{State}, P={Probability}, from {Previous}")]
    sealed class ChainedStateProbability : StateProbability
    {
        /// <summary>
        /// The previous state probability
        /// </summary>
        [CanBeNull]
        public ChainedStateProbability Previous { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateProbability" /> struct.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="probability">The probability.</param>
        /// <param name="previous">The previous.</param>
        /// <exception cref="System.ArgumentNullException">state
        /// or
        /// probability</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public ChainedStateProbability([NotNull] IState state, double probability, ChainedStateProbability previous = null)
            : base(state, probability)
        {
            Previous = previous;
        }
    }
}
