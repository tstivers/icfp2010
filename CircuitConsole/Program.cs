using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Circuits;


// INPUT STREAM: 01202101210201202
// KEY STREAM:   11021210112101221

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
            
            string inputstream;
            string outputstream;
            Circuit circuit;
            string function = file.ReadLine();
            switch (function)
            {
                case "FindInput":
                    inputstream = file.ReadLine();
                    circuit = CircuitUtility.BuildCircuit(file.ReadToEnd());
                    FindCircuitInput(circuit, inputstream);
                    break;
                case "FindOutput":
                    outputstream = file.ReadLine();
                    circuit = CircuitUtility.BuildCircuit(file.ReadToEnd());
                    FindCircuitOutput(circuit, outputstream);
                    break;
                case "FindCircuit":
                    inputstream = file.ReadLine();
                    outputstream = file.ReadLine();
                    int nodeCount = Int32.Parse(file.ReadLine());
                    int maxNodes = Int32.Parse(file.ReadLine());
                    FindCircuit(inputstream.ToStream(), outputstream.ToStream(), nodeCount, maxNodes);
                    break;
                default:
                    System.Console.WriteLine("ERROR: \"{0}\" - Unknown Function", function);
                    break;
            }

            System.Console.WriteLine(" ------------ execution ended --------------");
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
                    System.Console.WriteLine("possible inputstream: {0}", inputstream.FromStream());
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

        static int itercount;

        static void FindCircuit(int[] inputstream, int[] outputstream, int nodeCount, int maxNodes)
        {            
            while(nodeCount <= maxNodes)
            {
                Circuit c = new Circuit(nodeCount);

                for (int i = 0; i < nodeCount; i++)                
                    c.AddGate();

                IterateCircuit(c, 0, inputstream, outputstream);
                System.Console.WriteLine("nodes: {0}  iterations: {1}", nodeCount, itercount);
                nodeCount++;
                itercount = 0;
            }
        }

        static void IterateCircuit(Circuit c, int start, int[] inputstream, int[] outputstream)
        {
            for (int end = 0; end < c.Inputs.Count; end++)
            {
                if (c.Inputs[end].Source == null)
                {
                    c.Outputs[start].ConnectTo(c.Inputs[end]);
                    if (start == c.Outputs.Count - 1)
                        CheckCircuit(c, inputstream, outputstream);
                    else
                        IterateCircuit(c, start + 1, inputstream, outputstream);
                    c.Outputs[start].ConnectTo(null);
                }
            }
        }

        static void CheckCircuit(Circuit c, int[] inputstream, int[] outputstream)
        {
            //System.Console.WriteLine("Checking:\n" + c + "\n");
            foreach (GateOutput o in c.Outputs)
            {
                GateInput i = o.Target;
                c.OutputStream.ConnectTo(o);
                c.InputStream.ConnectTo(i);                
                if (c.Evaluate(inputstream, outputstream))
                {                    
                    System.Console.WriteLine("MATCHING CIRCUIT FOUND!!!\n{0}", c.DumpCircuit());
                }
                i.ConnectTo(o);
                itercount++;
            }
        }
    }
}
