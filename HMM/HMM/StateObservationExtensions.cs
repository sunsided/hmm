using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    /// <summary>
    /// Class StateObservationExtensions.
    /// </summary>
    static class StateObservationExtensions
    {
        /// <summary>
        /// Gets a <see cref="LabeledObservation"/> containing the <paramref name="state"/> seen as the given <seealso cref="observation"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <returns>StateObservationPair.</returns>
        public static LabeledObservation From([NotNull] this IState state, [NotNull] IObservation observation)
        {
            return new LabeledObservation(state, observation);
        }

        /// <summary>
        /// Gets a <see cref="LabeledObservation"/> containing the <paramref name="state"/> seen as the given <seealso cref="observation"/>.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <returns>StateObservationPair.</returns>
        public static LabeledObservation As([NotNull] this IObservation observation, [NotNull] IState state)
        {
            return new LabeledObservation(state, observation);
        }
    }
}
