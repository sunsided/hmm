using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace widemeadows.machinelearning.HMM
{
    /// <summary>
    /// Class TransitionMatrix.
    /// </summary>
    sealed class TransitionMatrix : IndexedStateMatrixBase
    {
        /// <summary>
        /// The probabilities
        /// </summary>
        [NotNull]
        private readonly double[,] _probabilities;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionMatrix"/> class.
        /// </summary>
        /// <param name="states">The number of states.</param>
        public TransitionMatrix([NotNull] IEnumerable<IState> states) 
            : base(states)
        {
            var count = States;
            _probabilities = new double[count, count];
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of going from the current state to the next state.
        /// </summary>
        /// <param name="currentState">The current state's index.</param>
        /// <param name="nextState">The next state's index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">currentState;The current state's index is out of range
        /// or
        /// nextState;The next state's index is out of range</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public double this[[NotNull] IState currentState, [NotNull] IState nextState]
        {
            [Pure]
            get { return GetTransition(currentState, nextState); }
            set { SetTransition(currentState, nextState, value); }
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of going from the current state to the next state.
        /// </summary>
        /// <param name="currentState">The current state's index.</param>
        /// <param name="nextState">The next state's index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// currentState;The current state's index is out of range
        /// or
        /// nextState;The next state's index is out of range
        /// </exception>
        [Pure]
        public double GetTransition([NotNull] IState currentState, [NotNull] IState nextState)
        {
            var ci = GetStateIndex(currentState);
            var ni = GetStateIndex(nextState);
            return GetTransition(ci, ni);
        }

        /// <summary>
        /// Sets the <see cref="System.Double" /> probability of going from the current state to the next state.
        /// </summary>
        /// <param name="currentState">The current state's index.</param>
        /// <param name="nextState">The next state's index.</param>
        /// <param name="probability">The transition probability.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public void SetTransition([NotNull] IState currentState, [NotNull] IState nextState, double probability)
        {
            if (probability < 0 || probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "The probability value must be in range 0..1");
            if (Double.IsNaN(probability) || Double.IsInfinity(probability)) throw new NotFiniteNumberException("The value must be a finite number.", probability);

            var ci = GetStateIndex(currentState);
            var ni = GetStateIndex(nextState);
            SetTransition(ci, ni, probability);
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of going from the current state to the next state.
        /// </summary>
        /// <param name="currentState">The current state's index.</param>
        /// <param name="nextState">The next state's index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// currentState;The current state's index is out of range
        /// or
        /// nextState;The next state's index is out of range
        /// </exception>
        [Pure]
        private double GetTransition(int currentState, int nextState)
        {
            if (currentState < 0 || currentState >= States) throw new ArgumentOutOfRangeException("currentState", currentState, "The current state's index is out of range");
            if (nextState < 0 || nextState >= States) throw new ArgumentOutOfRangeException("nextState", nextState, "The next state's index is out of range");

            return _probabilities[currentState, nextState];
        }

        /// <summary>
        /// Sets the <see cref="System.Double" /> probability of going from the current state to the next state.
        /// </summary>
        /// <param name="currentState">The current state's index.</param>
        /// <param name="nextState">The next state's index.</param>
        /// <param name="probability">The transition probability.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        private void SetTransition(int currentState, int nextState, double probability)
        {
            if (probability < 0 || probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "The probability value must be in range 0..1");
            if (Double.IsNaN(probability) || Double.IsInfinity(probability)) throw new NotFiniteNumberException("The value must be a finite number.", probability);
            _probabilities[currentState, nextState] = probability;
        }
    }
}
