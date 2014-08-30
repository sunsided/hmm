using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using widemeadows.machinelearning.HMM;

namespace widemeadows.machinelearning
{
    static class Program
    {
        static void Main(string[] args)
        {
            KillerClownProgram.Run();
            ArbitraryValueProgram.Run();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Applies the Viterbi algorithm and prints the most likely state sequence;
        /// </summary>
        /// <param name="hmm">The HMM.</param>
        /// <param name="observationSequence">The observation sequence.</param>
        internal static void ApplyViterbiAndPrint([NotNull] this HiddenMarkovModel hmm, [NotNull] IList<IObservation> observationSequence)
        {
            var stateSequence = hmm.Viterbi(observationSequence);

            // zip for output
            var taggedSequence = observationSequence.Zip(stateSequence, (o, s) => String.Format("{0}/{1} ({2:P})", o, s.State, s.Probability));

            // merge zipped sequence to string
            var sb = new StringBuilder();
            sb.AppendLine("Results:");
            foreach (var s in taggedSequence)
            {
                sb.Append("  ");
                sb.AppendLine(s);
            }
            
            // printify
            Console.WriteLine(sb.ToString());
            Console.WriteLine();
        }
    }
}
