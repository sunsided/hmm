using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NER.HMM;

namespace NER
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://www.cs.sfu.ca/~anoop/teaching/CMPT-413-Spring-2014/index.html

            const double compareEpsilon = 0.000001D;

            var states = new Registry<IState>();
            var adjective = states.Add(new NamedState("A"));
            var noun = states.Add(new NamedState("N"));

            var observations = new Registry<IObservation>();
            var clown = observations.Add(new NamedObservation("clown"));
            var killer = observations.Add(new NamedObservation("killer"));
            var crazy = observations.Add(new NamedObservation("crazy"));
            var problem = observations.Add(new NamedObservation("problem"));

            // test the HMM with a known graph
            {
                var initial = new InitialStateMatrix(states);
                initial.SetProbability(adjective, 1/3D);
                initial.SetProbability(noun, 2/3D);

                var transitions = new TransitionMatrix(states);
                transitions.SetTransition(adjective, adjective, 0);
                transitions.SetTransition(adjective, noun, 1);
                transitions.SetTransition(noun, adjective, 0.5);
                transitions.SetTransition(noun, noun, 0.5);

                var emissions = new EmissionMatrix(states, observations);
                emissions.SetEmission(adjective, clown, 0);
                emissions.SetEmission(adjective, killer, 0);
                emissions.SetEmission(adjective, problem, 0);
                emissions.SetEmission(adjective, crazy, 1);
                emissions.SetEmission(noun, clown, 0.4);
                emissions.SetEmission(noun, killer, 0.3);
                emissions.SetEmission(noun, problem, 0.3);
                emissions.SetEmission(noun, crazy, 0);

                var hmm = new HiddenMarkovModel(states.AsReadOnly(), initial, transitions, emissions);

                // P(AA | killer clown)
                var paa = hmm.GetProbability(killer.As(adjective), clown.As(adjective));
                Debug.Assert(Math.Abs(paa) < compareEpsilon);

                // P(AN | killer clown)
                var pan = hmm.GetProbability(killer.As(adjective), clown.As(noun));
                Debug.Assert(Math.Abs(pan) < compareEpsilon);

                // P(NN | killer clown)
                var pnn = hmm.GetProbability(killer.As(noun), clown.As(noun));
                Debug.Assert(Math.Abs(0.04 - pnn) < compareEpsilon);

                // P(NA | killer clown)
                var pna = hmm.GetProbability(killer.As(noun), clown.As(adjective));
                Debug.Assert(Math.Abs(pna) < compareEpsilon);
            }

            // test supervised learning of the HMM
            {
                var trainingSet = new List<IList<LabeledObservation>>
                {
                    new[] {killer.As(noun), clown.As(noun)},
                    new[] {killer.As(noun), problem.As(noun)},
                    new[] {crazy.As(adjective), problem.As(noun)},
                    new[] {crazy.As(adjective), clown.As(noun)},
                    new[] {problem.As(noun), crazy.As(adjective), clown.As(noun)},
                    new[] {clown.As(noun), crazy.As(adjective), killer.As(noun)},
                };

                // learn the initial state probabilities
                var initial = new InitialStateMatrix(states);
                initial.Learn(trainingSet);

                // learn the transition probabilities
                var transitions = new TransitionMatrix(states);
                transitions.Learn(trainingSet);

                // learn the emission probabilities
                var emissions = new EmissionMatrix(states, observations);
                emissions.Learn(trainingSet);

                var hmm = new HiddenMarkovModel(states.AsReadOnly(), initial, transitions, emissions);

                // P(AA | killer clown)
                var paa = hmm.GetProbability(killer.As(adjective), clown.As(adjective));
                Debug.Assert(Math.Abs(paa) < compareEpsilon);

                // P(AN | killer clown)
                var pan = hmm.GetProbability(killer.As(adjective), clown.As(noun));
                Debug.Assert(Math.Abs(pan) < compareEpsilon);

                // P(NN | killer clown)
                var pnn = hmm.GetProbability(killer.As(noun), clown.As(noun));
                Debug.Assert(Math.Abs(0.04 - pnn) < compareEpsilon);

                // P(NA | killer clown)
                var pna = hmm.GetProbability(killer.As(noun), clown.As(adjective));
                Debug.Assert(Math.Abs(pna) < compareEpsilon);

                // apply the viterbi algorithm to find the most likely sequence
                ApplyViterbiAndPrint(hmm, new[] { killer, crazy, clown, problem });
                ApplyViterbiAndPrint(hmm, new[] { crazy, killer, clown, problem });
                ApplyViterbiAndPrint(hmm, new[] { crazy, clown, killer, crazy, problem });

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Applies the Viterbi algorithm and prints the most likely state sequence;
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        /// <param name="observationSequence">The observation sequence.</param>
        private static void ApplyViterbiAndPrint([NotNull] HiddenMarkovModel hmm, [NotNull] IList<IObservation> observationSequence)
        {
            var stateSequence = hmm.Viterbi(observationSequence);

            // zip for output
            var taggedSequence = observationSequence.Zip(stateSequence, (o, s) => String.Format("{0}/{1}", o, s));

            // merge zipped sequence to string
            var sb = new StringBuilder();
            foreach (var s in taggedSequence)
            {
                sb.Append(s);
                sb.Append(" ");
            }
            
            // printify
            Console.WriteLine(sb.ToString().TrimEnd());
        }
    }
}
