using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace NER.HMM
{
    /// <summary>
    /// Struct StateProbability
    /// </summary>
    [DebuggerDisplay("{State}, P={Probability}")]
    struct StateProbability
    {
        /// <summary>
        /// The state
        /// </summary>
        [NotNull]
        public readonly IState State;

        /// <summary>
        /// The probability
        /// </summary>
        public readonly double Probability;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateProbability"/> struct.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="probability">The probability.</param>
        /// <exception cref="System.ArgumentNullException">
        /// state
        /// or
        /// probability
        /// </exception>
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
