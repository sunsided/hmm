using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace NER.HMM
{
    /// <summary>
    /// Class InitialStateMatrix.
    /// </summary>
    sealed class InitialStateMatrix : IndexedStateMatrixBase
    {
        /// <summary>
        /// The probabilities
        /// </summary>
        [NotNull]
        private readonly double[] _probabilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="InitialStateMatrix"/> class.
        /// </summary>
        /// <param name="states">The number of states.</param>
        public InitialStateMatrix([NotNull] IEnumerable<IState> states) 
            : base(states)
        {
            var count = States;
            _probabilities = new double[count];
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of the state's emission at begin of the sequence.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">currentState;The state's index is out of range</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public double this[[NotNull] IState state]
        {
            [Pure]
            get { return GetProbability(state); }
            set { SetProbability(state, value); }
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of emitting the state at the begin of the sequence.
        /// <para>
        /// This function is an alias for <see cref="GetProbability"/>.
        /// </para>
        /// </summary>
        /// <param name="state">The state's index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// currentState;The current state's index is out of range
        /// </exception>
        /// <seealso cref="GetProbability"/>
        [Pure]
        public double Generate([NotNull] IState state)
        {
            return GetProbability(state);
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of emitting the state at the begin of the sequence.
        /// </summary>
        /// <param name="state">The state's index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// currentState;The current state's index is out of range
        /// </exception>
        [Pure]
        public double GetProbability([NotNull] IState state)
        {
            var index = GetStateIndex(state);
            return GetProbability(index);
        }

        /// <summary>
        /// Sets the <see cref="System.Double" /> probability of emitting the state at the begin of the sequence.
        /// </summary>
        /// <param name="state">The state's index.</param>
        /// <param name="probability">The transition probability.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public void SetProbability([NotNull] IState state, double probability)
        {
            if (probability < 0 || probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "The probability value must be in range 0..1");
            if (Double.IsNaN(probability) || Double.IsInfinity(probability)) throw new NotFiniteNumberException("The value must be a finite number.", probability);

            var index = GetStateIndex(state);
            SetProbability(index, probability);
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of emitting the state at the begin of the sequence.
        /// </summary>
        /// <param name="state">The state's index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// currentState;The state's index is out of range
        /// </exception>
        [Pure]
        private double GetProbability(int state)
        {
            if (state < 0 || state >= States) throw new ArgumentOutOfRangeException("state", state, "The state's index is out of range");
            return _probabilities[state];
        }

        /// <summary>
        /// Sets the <see cref="System.Double" /> probability of emitting the state at the begin of the sequence.
        /// </summary>
        /// <param name="state">The  state's index.</param>
        /// <param name="probability">The transition probability.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        private void SetProbability(int state, double probability)
        {
            if (probability < 0 || probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "The probability value must be in range 0..1");
            if (Double.IsNaN(probability) || Double.IsInfinity(probability)) throw new NotFiniteNumberException("The value must be a finite number.", probability);
            _probabilities[state] = probability;
        }

        /// <summary>
        /// Learns the initial probabilities from the specified training set.
        /// </summary>
        /// <param name="trainingSet">The training set.</param>
        public void Learn([NotNull] IList<IList<LabeledObservation>> trainingSet)
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
                var probability = (double) occurrences/totalCount;

                SetProbability(state, probability);
            }
        }
    }
}
