using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NER.HMM
{
    internal class IndexedStateMatrixBase
    {
        /// <summary>
        /// The number of states
        /// </summary>
        private readonly int _stateCount;

        /// <summary>
        /// The state dictionary
        /// </summary>
        [NotNull]
        private readonly Dictionary<IState, int> _states = new Dictionary<IState, int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransitionMatrix"/> class.
        /// </summary>
        /// <param name="states">The states.</param>
        public IndexedStateMatrixBase([NotNull] IEnumerable<IState> states)
        {
            var count = 0;
            foreach (var state in states)
            {
                var index = count++;
                _states.Add(state, index);
            }

            _stateCount = count;
        }

        /// <summary>
        /// Gets the number of states.
        /// </summary>
        /// <value>The states.</value>
        public int States { [Pure] get { return _stateCount; } }

        /// <summary>
        /// Gets the index of the state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.ArgumentException">The given state was not previously registered;state</exception>
        [Pure]
        protected int GetStateIndex([NotNull] IState state)
        {
            int index;
            if (_states.TryGetValue(state, out index)) return index;
            throw new ArgumentException("The given state was not previously registered", "state");
        }
    }
}