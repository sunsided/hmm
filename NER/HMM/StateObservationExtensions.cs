using JetBrains.Annotations;

namespace NER.HMM
{
    /// <summary>
    /// Class StateObservationExtensions.
    /// </summary>
    static class StateObservationExtensions
    {
        /// <summary>
        /// Gets a <see cref="StateObservationPair"/> containing the <paramref name="state"/> seen as the given <seealso cref="observation"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <returns>StateObservationPair.</returns>
        public static StateObservationPair From([NotNull] this IState state, [NotNull] IObservation observation)
        {
            return new StateObservationPair(state, observation);
        }

        /// <summary>
        /// Gets a <see cref="StateObservationPair"/> containing the <paramref name="state"/> seen as the given <seealso cref="observation"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <returns>StateObservationPair.</returns>
        public static StateObservationPair As([NotNull] this IObservation observation, [NotNull] IState state)
        {
            return new StateObservationPair(state, observation);
        }
    }
}
