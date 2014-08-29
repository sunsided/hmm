using JetBrains.Annotations;

namespace NER.HMM
{
    sealed class HiddenMarkovModel
    {
        /// <summary>
        /// The inital state matrix
        /// </summary>
        [NotNull]
        private readonly InitialStateMatrix _inital;

        /// <summary>
        /// The transmission matrix
        /// </summary>
        [NotNull]
        private readonly TransitionMatrix _transmission;

        /// <summary>
        /// The emission matrix
        /// </summary>
        [NotNull]
        private readonly EmissionMatrix _emission;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiddenMarkovModel"/> class.
        /// </summary>
        /// <param name="inital">The inital state matrix.</param>
        /// <param name="transmission">The transmission matrix.</param>
        /// <param name="emission">The emission matrix.</param>
        public HiddenMarkovModel(
            [NotNull] InitialStateMatrix inital, 
            [NotNull] TransitionMatrix transmission,
            [NotNull] EmissionMatrix emission)
        {
            _inital = inital;
            _transmission = transmission;
            _emission = emission;
        }

        /// <summary>
        /// Gets the probability.
        /// </summary>
        /// <param name="left">The left state as seen with the observation.</param>
        /// <param name="right">The right state as seen with the observation.</param>
        /// <returns>System.Double.</returns>
        public double GetProbability(
            StateObservationPair left,
            StateObservationPair right)
        {
            var p = _inital.GetProbability(left.State)*
                    _emission.GetEmission(left)*
                    _transmission.GetTransition(left.State, right.State)*
                    _emission.GetEmission(right);
            return p;
        }
    }
}
