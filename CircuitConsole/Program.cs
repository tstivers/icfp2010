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
                    FindCircuit(inputstream.ToStream(), outputstream.ToStream());
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

        static void FindCircuit(int[] inputstream, int[] outputstream)
        {
            int nodeCount = 1;

            while(true)
            {
                Circuit c = new Circuit();

                for (int i = 0; i < nodeCount; i++)
                {
                    Gate g = c.AddGate(i);
                    g.OutputL.ConnectTo(g.InputL);
                    g.OutputR.ConnectTo(g.InputR);
                }

                //TODO: swap wires around here

                //for (int iw = 0; iw < c.Wires.Count; iw++)
                //{
                //    GateInput input = c.Wires[iw].End;
                //    GateOutput output = c.Wires[iw].Start;

                //    input.ConnectTo(c.InputStream);
                //    output.ConnectTo(c.OutputStream);

                //    //c.Evaluate(inputstream, outputstream);

                //    input.ConnectTo(output);
                //}

                //for(int ic = 0; ic < nodeCount * 2; ic++)
                //{
                //    c.RemoveWires();

                //    c.Gates[ic / 2].Inputs[ic % 2].ConnectTo(c.InputStream);
                //}
            }
        }

        static void CheckCircuit(Circuit c, GateOutput current)
        {
            foreach (Gate g in c.Gates)
            {
                //GateInput imput = g.OutputL.Wire.
            }
        }
    }
}
