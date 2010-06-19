using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Circuits;

namespace CircuitConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("ERROR: no input file specified");
                System.Console.ReadKey(true);
                return;
            }

            var file = File.OpenText(args[0]);

            string function = file.ReadLine();
            string stream = file.ReadLine();

            var circuit = CircuitUtility.BuildCircuit(file.ReadToEnd());

            switch (function)
            {
                case "FindInput":
                    FindCircuitInput(circuit, stream);
                    break;
                case "FindOutput":
                    FindCircuitOutput(circuit, stream);
                    break;
                default:
                    System.Console.WriteLine("ERROR: \"{0}\" - Unknown Function", function);
                    break;
            }

            System.Console.ReadKey(true);
        }

        static void FindCircuitOutput(Circuit circuit, string inputstream)
        {
            System.Console.WriteLine("finding output for circuit: \n\n{0}\n", circuit.DumpCircuit());
            System.Console.WriteLine("inputstream: {0}", inputstream);

            var outputstream = circuit.Evaluate(inputstream);

            System.Console.WriteLine("outputstream: {0}", outputstream);            
        }

        static void FindCircuitInput(Circuit circuit, string outputstream)
        {
            System.Console.WriteLine("finding possible inputs for circuit: \n\n{0}\n", circuit.DumpCircuit());
            System.Console.WriteLine("outputstream: {0}", outputstream);

            int[] inputstream = new int[outputstream.Length];
            for (int i = 0; i < 3; i++)
            {
                inputstream[0] = i;
                CheckInput(circuit, inputstream, outputstream.ToStream(), 1);
            }
        }

        static bool CheckInput(Circuit circuit, int[] inputstream, int[] outputstream, int length)
        {
            int[] testoutput = circuit.Evaluate(inputstream.Take(length).ToArray());

            //System.Console.WriteLine("tested input {0}, got {1}", inputstream.Take(length).ToArray().FromStream(), testoutput.FromStream());

            // sequence matched!
            if (testoutput.SequenceEqual(outputstream.Take(length)))
            {
                if (testoutput.Length == outputstream.Length) // found a complete match
                    System.Console.WriteLine("found matching inputstream: {0}", inputstream.FromStream());
                else
                {
                    for (int i = 0; i < 3; i++)
                    {
                        inputstream[length] = i;
                        CheckInput(circuit, inputstream, outputstream, length + 1);
                    }
                }
            }
            
            // sequence did not match
            return false;
        }
    }
}
