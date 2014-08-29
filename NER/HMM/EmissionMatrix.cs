using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace NER.HMM
{
    /// <summary>
    /// Class EmissionMatrix.
    /// </summary>
    sealed class EmissionMatrix : IndexedStateMatrixBase
    {
        /// <summary>
        /// The probabilities
        /// </summary>
        [NotNull]
        private readonly double[,] _probabilities;

        /// <summary>
        /// The number of observations
        /// </summary>
        private readonly int _observationCount;

        /// <summary>
        /// The observation dictionary
        /// </summary>
        [NotNull]
        private readonly Dictionary<IObservation, int> _observations = new Dictionary<IObservation, int>();

        /// <summary>
        /// Gets the number of observations.
        /// </summary>
        /// <value>The observations.</value>
        public int Observations { [Pure] get { return _observationCount; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionMatrix" /> class.
        /// </summary>
        /// <param name="states">The states.</param>
        /// <param name="observations">The observations.</param>
        public EmissionMatrix([NotNull] IEnumerable<IState> states, [NotNull] IEnumerable<IObservation> observations) 
            : base(states)
        {
            var count = 0;
            foreach (var observation in observations)
            {
                var index = count++;
                _observations.Add(observation, index);
            }

            _observationCount = count;
            var stateCount = States;
            _probabilities = new double[stateCount, _observationCount];
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of the state's emission.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">currentState;The state's index is out of range</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public double this[[NotNull] IState state, [NotNull] IObservation observation]
        {
            [Pure]
            get { return GetEmission(state, observation); }
            set { SetEmission(state, observation, value); }
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of emitting the state.
        /// <para>
        /// This function is an alias for <see cref="GetEmission"/>.
        /// </para>
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="observation">The observation.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// currentState;The current state's index is out of range
        /// </exception>
        /// <seealso cref="GetEmission"/>
        [Pure]
        public double Generate([NotNull] IState state, [NotNull] IObservation observation)
        {
            return GetEmission(state, observation);
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of emitting the state.
        /// </summary>
        /// <param name="pair">The state and observation pair.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentException">The given state was not previously registered;state</exception>
        /// <exception cref="System.ArgumentException">The given observation was not previously registered;observation</exception>
        [Pure, DebuggerStepThrough]
        public double GetEmission(StateObservationPair pair)
        {
            return GetEmission(pair.State, pair.Observation);
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of emitting the state.
        /// </summary>
        /// <param name="state">The state's index.</param>
        /// <param name="observation">The observation's index.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentException">The given state was not previously registered;state</exception>
        /// <exception cref="System.ArgumentException">The given observation was not previously registered;observation</exception>
        [Pure]
        public double GetEmission([NotNull] IState state, [NotNull] IObservation observation)
        {
            var si = GetStateIndex(state);
            var oi = GetObservationIndex(observation);
            return GetEmission(state:si, observation:oi);
        }

        /// <summary>
        /// Sets the <see cref="System.Double" /> probability of emitting the state.
        /// </summary>
        /// <param name="state">The state's index.</param>
        /// <param name="observation">The observation.</param>
        /// <param name="probability">The transition probability.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        public void SetEmission([NotNull] IState state, [NotNull] IObservation observation, double probability)
        {
            if (probability < 0 || probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "The probability value must be in range 0..1");
            if (Double.IsNaN(probability) || Double.IsInfinity(probability)) throw new NotFiniteNumberException("The value must be a finite number.", probability);

            var si = GetStateIndex(state);
            var oi = GetObservationIndex(observation);
            SetEmission(state: si, observation:oi, probability:probability);
        }

        /// <summary>
        /// Gets the <see cref="System.Double" /> probability of emitting the state.
        /// </summary>
        /// <param name="state">The state's index.</param>
        /// <param name="observation">The observation's omdex.</param>
        /// <returns>System.Double.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">currentState;The state's index is out of range</exception>
        [Pure]
        private double GetEmission(int state, int observation)
        {
            if (state < 0 || state >= States) throw new ArgumentOutOfRangeException("state", state, "The state's index is out of range");
            return _probabilities[state, observation];
        }

        /// <summary>
        /// Sets the <see cref="System.Double" /> probability of emitting the state.
        /// </summary>
        /// <param name="state">The  state's index.</param>
        /// <param name="observation">The observation's index.</param>
        /// <param name="probability">The transition probability.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">probability;The probability value must be in range 0..1</exception>
        /// <exception cref="System.NotFiniteNumberException">The value must be a finite number.</exception>
        private void SetEmission(int state, int observation, double probability)
        {
            if (probability < 0 || probability > 1) throw new ArgumentOutOfRangeException("probability", probability, "The probability value must be in range 0..1");
            if (Double.IsNaN(probability) || Double.IsInfinity(probability)) throw new NotFiniteNumberException("The value must be a finite number.", probability);
            _probabilities[state, observation] = probability;
        }

        /// <summary>
        /// Gets the index of the state.
        /// </summary>
        /// <param name="observation">The observation.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentException">The given observation was not previously registered;observation</exception>
        [Pure]
        private int GetObservationIndex([NotNull] IObservation observation)
        {
            int index;
            if (_observations.TryGetValue(observation, out index)) return index;
            throw new ArgumentException("The given observation was not previously registered", "observation");
        }
    }
}
