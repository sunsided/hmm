using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    /// <summary>
    /// Class ClassicalBaumWelch. This class cannot be inherited.
    /// </summary>
    sealed class ClassicalBaumWelchLearning
    {
        /// <summary>
        /// Learns the parameters for the given system.
        /// </summary>
        /// <param name="initial">The initial.</param>
        /// <param name="transition">The transition.</param>
        /// <param name="emission">The emission.</param>
        /// <param name="trainingSet">The training set.</param>
        public void Learn([NotNull] InitialStateMatrix initial, [NotNull] TransitionMatrix transition,
            [NotNull] EmissionMatrix emission,
            [NotNull] IList<IList<LabeledObservation>> trainingSet)
        {
            Learn(initial, trainingSet);
            Learn(transition, trainingSet);
            Learn(emission, trainingSet);
        }

        /// <summary>
        /// Learns the initial probabilities from the specified training set.
        /// </summary>
        /// <param name="initial">The initial state matrix.</param>
        /// <param name="trainingSet">The training set.</param>
        private void Learn([NotNull] InitialStateMatrix initial, [NotNull] IList<IList<LabeledObservation>> trainingSet)
        {
            // group all traning sentences beginning with the same state
            var groups = trainingSet.GroupBy(set => set.First().State).ToList();
            var totalCount = trainingSet.Count;

            // get the probability by dividing the count of each state's occurrence
            // by the total number of training sets
            foreach (var group in groups)
            {
                var state = group.Key;
                var occurrences = group.Count();
                var probability = (double)occurrences / totalCount;

                initial.SetProbability(state, probability);
            }
        }

        /// <summary>
        /// Learns the transition probabilities from the specified training set.
        /// </summary>
        /// <param name="transition">The transition matrix.</param>
        /// <param name="trainingSet">The training set.</param>
        private void Learn([NotNull] TransitionMatrix transition, [NotNull] IEnumerable<IList<LabeledObservation>> trainingSet)
        {
            // generate bigrams from each training sentence and
            // group these pairs by the left item's state
            var pairsGroupedByLeftState = trainingSet
                .SelectMany(set => set.Pairwise())
                .GroupBy(pair => pair.Left.State);

            // for each left state, group the items by the right states
            foreach (var pairsOfSameStartingClass in pairsGroupedByLeftState)
            {
                var leftState = pairsOfSameStartingClass.Key;
                var pairsOfSameStartingStateGroupedByRightState =
                    pairsOfSameStartingClass.GroupBy(pair => pair.Right.State);
                var countOfSameStartingState = pairsOfSameStartingClass.Count();

                // determine each probability P(left -> right) by dividing
                // the occurrence count of each right state by the total number of all
                // occurrences starting in the left state
                foreach (var grouping in pairsOfSameStartingStateGroupedByRightState)
                {
                    var rightState = grouping.Key;
                    var occurrences = grouping.Count();
                    var percentage = (double)occurrences / countOfSameStartingState;

                    transition.SetTransition(leftState, rightState, percentage);
                }
            }
        }

        /// <summary>
        /// Learns the emission probabilities from the specified training set.
        /// </summary>
        /// <param name="emission">The emission.</param>
        /// <param name="trainingSet">The training set.</param>
        private void Learn([NotNull] EmissionMatrix emission, [NotNull] IEnumerable<IList<LabeledObservation>> trainingSet)
        {
            var observationsGroupedByState = trainingSet
                .SelectMany(set => set)
                .GroupBy(set => set.State);

            foreach (var example in observationsGroupedByState)
            {
                var state = example.Key;
                var examples = example.GroupBy(e => e.Observation).ToList();
                var totalCount = examples.Sum(e => e.Count());

                foreach (var group in examples)
                {
                    var observation = group.Key;
                    var count = group.Count();
                    var probability = (double)count / totalCount;

                    emission.SetEmission(state, observation, probability);
                }
            }
        }
    }
}
