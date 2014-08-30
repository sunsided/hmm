using widemeadows.machinelearning.HMM;

namespace widemeadows.machinelearning
{
    class ArbitraryValueProgram
    {
        internal static void Run()
        {
            // http://ocw.mit.edu/courses/aeronautics-and-astronautics/16-410-principles-of-autonomy-and-decision-making-fall-2010/lecture-notes/MIT16_410F10_lec21.pdf

            var states = new Registry<IState>();
            var x1 = states.Add(new NamedState("x1"));
            var x2 = states.Add(new NamedState("x2"));
            var x3 = states.Add(new NamedState("x3"));

            var observations = new Registry<IObservation>();
            var o2 = observations.Add(new NamedObservation("o2"));
            var o3 = observations.Add(new NamedObservation("o3"));

            // test the HMM with a known graph
            {
                var initial = new InitialStateMatrix(states);
                initial.SetProbability(x1, 1);
                initial.SetProbability(x2, 0);
                initial.SetProbability(x3, 0);

                var transitions = new TransitionMatrix(states);
                transitions.SetTransition(x1, x1, 0);
                transitions.SetTransition(x1, x2, 0.5);
                transitions.SetTransition(x1, x3, 0.5);
                transitions.SetTransition(x2, x1, 0);
                transitions.SetTransition(x2, x2, 0.9);
                transitions.SetTransition(x2, x3, 0.1);
                transitions.SetTransition(x3, x1, 0);
                transitions.SetTransition(x3, x2, 0);
                transitions.SetTransition(x3, x3, 1);

                var emissions = new EmissionMatrix(states, observations);
                emissions.SetEmission(x1, o2, 0.5);
                emissions.SetEmission(x2, o2, 0.9);
                emissions.SetEmission(x3, o2, 0.1);

                emissions.SetEmission(x1, o3, 0.5);
                emissions.SetEmission(x2, o3, 0.1);
                emissions.SetEmission(x3, o3, 0.9);

                var hmm = new HiddenMarkovModel(states, initial, transitions, emissions);

                // expected output: 1, 3, 3, 3, 3, 3, 3, 3, 3
                hmm.ApplyViterbiAndPrint(new[] { o2, o3, o3, o2, o2, o2, o3, o2, o3 });
                
                // expected output: 1, 2, 2, 2, 2, 2, 2, 2
                hmm.ApplyViterbiAndPrint(new[] { o2, o3, o3, o2, o2, o2, o3, o2});
            }
        }
    }
}
