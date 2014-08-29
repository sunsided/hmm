using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace NER.HMM
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
            var p = _inital.GetProbability(left.State)*
                    _emission.GetEmission(left)*
                    _transition.GetTransition(left.State, right.State)*
                    _emission.GetEmission(right);
            return p;
        }

        /// <summary>
        /// Determines the most likely sequence of states given a sequence of observations
        /// using the Viterbi algorithm.
        /// </summary>
        /// <param name="observations">The observations.</param>
        /// <returns>IEnumerable&lt;IState&gt;.</returns>
        [NotNull]
        public IEnumerable<IState> Viterbi([NotNull] IEnumerable<IObservation> observations)
        {
            var stateCount = _states.Count;
            var viterbiTable = new List<List<StateProbability>>();
            bool isFirst = true;

            List<StateProbability> previousRound = null;
            List<StateProbability> currentRound = null;

            foreach (var observation in observations)
            {
                previousRound = currentRound;
                currentRound = new List<StateProbability>(stateCount);
                viterbiTable.Add(currentRound);

                // initialize the first round
                if (isFirst)
                {
                    // for each possible state, calculate the
                    // initial probability given the observation.
                    currentRound.AddRange(from state in _states
                        let p = _inital.GetProbability(state)*_emission.GetEmission(state, observation)
                        select new StateProbability(state, p)
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
                        let probability =
                            previousState.Probability*_transition.GetTransition(previousState.State, s)*
                            _emission.Generate(s, o)
                        select new StateProbability(currentState, probability, previousState)
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
                yield return stack.Pop().State;
            }
        }
    }
}
