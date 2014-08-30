using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    sealed class HiddenMarkovModel
    {
        /// <summary>
        /// The states
        /// </summary>
        [NotNull]
        private readonly ReadOnlyCollection<IState> _states;

        /// <summary>
        /// The inital state matrix
        /// </summary>
        [NotNull]
        private readonly InitialStateMatrix _inital;

        /// <summary>
        /// The transmission matrix
        /// </summary>
        [NotNull]
        private readonly TransitionMatrix _transition;

        /// <summary>
        /// The emission matrix
        /// </summary>
        [NotNull]
        private readonly EmissionMatrix _emission;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiddenMarkovModel" /> class.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <param name="inital">The inital state matrix.</param>
        /// <param name="transition">The transmission matrix.</param>
        /// <param name="emission">The emission matrix.</param>
        public HiddenMarkovModel(
            [NotNull] ReadOnlyCollection<IState> states,
            [NotNull] InitialStateMatrix inital, 
            [NotNull] TransitionMatrix transition,
            [NotNull] EmissionMatrix emission)
        {
            _states = states;
            _inital = inital;
            _transition = transition;
            _emission = emission;
        }

        /// <summary>
        /// Gets the probability of a sequence starting with the <paramref name="left"/>
        /// hypothesis and ending with the <paramref name="right"/> one.
        /// </summary>
        /// <param name="left">The left state as seen with the observation.</param>
        /// <param name="right">The right state as seen with the observation.</param>
        /// <returns>System.Double.</returns>
        public double GetProbability(
            LabeledObservation left,
            LabeledObservation right)
        {
#if true
            var p = _inital.GetProbability(left.State)*
                    _emission.GetEmission(left)*
                    _transition.GetTransition(left.State, right.State)*
                    _emission.GetEmission(right);
#else
            // using Log-Likelihoods
            var p = Math.Exp(Math.Log(_inital.GetProbability(left.State)) +
                             Math.Log(_emission.GetEmission(left)) +
                             Math.Log(_transition.GetTransition(left.State, right.State)) +
                             Math.Log(_emission.GetEmission(right)));
            if (Double.IsInfinity(p)) return 0;
#endif
            return p;
        }

        /// <summary>
        /// Determines the most likely sequence of states given a sequence of observations
        /// using the Viterbi algorithm.
        /// </summary>
        /// <param name="observations">The observations.</param>
        /// <returns>IEnumerable&lt;IState&gt;.</returns>
        [NotNull]
        public IEnumerable<StateProbability> Viterbi([NotNull] IEnumerable<IObservation> observations)
        {
            var stateCount = _states.Count;
            var viterbiTable = new List<List<ChainedStateProbability>>();
            bool isFirst = true;

            List<ChainedStateProbability> currentRound = null;

            foreach (var observation in observations)
            {
                var previousRound = currentRound;
                currentRound = new List<ChainedStateProbability>(stateCount);
                viterbiTable.Add(currentRound);

                // initialize the first round
                if (isFirst)
                {
                    // for each possible state, calculate the
                    // initial probability given the observation.
                    currentRound.AddRange(from state in _states
                        let p = _inital.GetProbability(state)*_emission.GetEmission(state, observation)
                        select new ChainedStateProbability(state, p)
                        );

                    isFirst = false;
                    continue;
                }

                // for each state calculate the transition probability given
                // any previous state
                currentRound.AddRange(
                    from currentState in _states
                    let s = currentState
                    let o = observation
                    let bestMatch = (
                        from previousState in previousRound
                        let probability = previousState.Probability * _transition.GetTransition(previousState.State, s) * _emission.Generate(s, o)
                        select new ChainedStateProbability(currentState, probability, previousState)
                        ).OrderByDescending(p => p.Probability)
                        .First()
                    select bestMatch
                    );
            }

            Debug.Assert(currentRound != null, "currentRound != null");

            // backtrack by selecting the originator of the most probable result
            var stack = new Stack<StateProbability>();
            var token = currentRound.OrderByDescending(r => r.Probability).First();
            while (token != null)
            {
                stack.Push(token);
                token = token.Previous;
            }

            // emit backtracked path
            while (stack.Count > 0)
            {
                yield return stack.Pop();
            }
        }

        /// <summary>
        /// Calculates the probability that this model has generated the given sequence.
        /// </summary>
        /// <param name="observations">The observations.</param>
        /// <param name="logarithmic">if set to <coefficients>true</coefficients>, outputs log-likelihood, otherwise regular probability will be returned.</param>
        /// <returns>The probability that the given sequence has been generated by this model.</returns>
        /// <remarks>Evaluation problem. Given the HMM  M = (A, B, pi) and  the observation
        /// sequence O = {o1, o2, ..., oK}, calculate the probability that model
        /// M has generated sequence O. This can be computed efficiently using the
        /// either the Viterbi or the Forward algorithms.</remarks>
        public double Evaluate([NotNull] IEnumerable<IObservation> observations, bool logarithmic = false)
        {
            // calculate the forward probability
            IList<double> coefficients;
            ForwardProbability(observations.ToList(), out coefficients);

            // calculate the log-likelihood
            var likelihood = 0D;
            for (int i = 0; i < coefficients.Count; ++i)
            {
                likelihood += Math.Log(coefficients[i]);
            }

            // return the probability or log-likelihood
            return logarithmic ? likelihood : Math.Exp(likelihood);
        }

        /// <summary>
        /// Calculates the forward probability given the observations.
        /// </summary>
        /// <param name="observations">The observations.</param>
        /// <param name="coefficients">The coefficients.</param>
        /// <returns>System.Double.</returns>
        private void ForwardProbability([NotNull] IList<IObservation> observations, out IList<double> coefficients)
        {
            var states = _states;
            var emission = _emission;
            var transition = _transition;

            var observationCount = observations.Count;
            var stateCount = states.Count;

            // the array of forward probabilities
            var fwd = new double[observationCount, stateCount];

            // the array of summed probabilities per observation
            coefficients = new double[observationCount];

            // initialize initial probabilities
            var firstObservation = observations.First();
            for (int i = 0; i < stateCount; ++i)
            {
                var state = states[i];

                // calculate the probability of starting in that state given the first observation
                var probability = _inital.GetProbability(state) * emission.GetEmission(state, firstObservation);
                fwd[0, i] = probability;

                // sum probabilities in order to normalize them later
                coefficients[0] += probability;
            }

            // normalize initial probabilities
            if (coefficients[0] > 0)
            {
                var normalizationFactor = 1.0D/coefficients[0];
                for (int i = 0; i < stateCount; ++i)
                {
                    fwd[0, i] = fwd[0, i] * normalizationFactor;
                }
            }

            // induce the remaining probabilities by iterating
            // over all time step's observations
            for (int t = 1; t < observationCount; ++t)
            {
                var observation = observations[t];

                // loop through all states
                for (int i = 0; i < stateCount; ++i)
                {
                    var state = states[i];

                    // get the probability of emitting the observation
                    // given the current state
                    var emissionProbability = emission.GetEmission(state, observation);

                    // sum all the probabilities of getting to this state
                    // from any other (previous) state 
                    // given the probability of having been in that previous state.
                    var summedProbabilities = 0D;
                    for (int j = 0; j < stateCount; ++j)
                    {
                        var previousState = states[j];
                        var previousProbability = fwd[t - 1, j];
                        var transitionProbability = transition.GetTransition(previousState, state);
                        summedProbabilities += previousProbability * transitionProbability;
                    }

                    // calculate the forward probability for this time step
                    var probability = emissionProbability*summedProbabilities;
                    fwd[t, i] = probability;

                    // sum probabilities in order to normalize them later
                    coefficients[t] += probability;
                }

                // normalize the current time step's probabilities
                if (coefficients[t] > 0)
                {
                    var normalizationFactor = 1.0D / coefficients[t];
                    for (int i = 0; i < stateCount; ++i)
                    {
                        fwd[t, i] = fwd[t, i] * normalizationFactor;
                    }
                }
            }

            // return the scaled forward probabilities
            // return fwd;
        }
    }
}
