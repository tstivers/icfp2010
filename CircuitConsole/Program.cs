using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Circuits;

namespace CircuitConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new Circuit();
            var gate0 = c.AddGate(0);

            gate0.InputL.ConnectTo(gate0.OutputR);
            gate0.InputR.ConnectTo(c.InputStream);
            gate0.OutputL.ConnectTo(c.OutputStream);

            System.Console.WriteLine("[Circuit]\n\n" + c.DumpCircuit() + "\n");            

            while (true)
            {
                int input = 0;
                System.Console.Write("Input: ");

                var key = System.Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        return;
                        break;
                    case ConsoleKey.D0:
                        break;
                    case ConsoleKey.D1:
                        input = 1;
                        break;
                    case ConsoleKey.D2:
                        input = 2;
                        break;
                    default:
                        System.Console.WriteLine("<expected 0,1,2,esc>");
                        continue;
                }

                System.Console.WriteLine("" + input);
                int output = c.Evaluate(input);
                System.Console.WriteLine("Output: " + output);               
            }
        }
    }
}
