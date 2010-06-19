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
            var gate1 = c.AddGate(1);
            
            gate0.InputL.ConnectTo(c.InputGate);
            gate0.InputR.ConnectTo(gate0.OutputR);
            gate0.OutputL.ConnectTo(gate1.InputL);

            gate1.InputR.ConnectTo(gate1.OutputL);
            gate1.OutputR.ConnectTo(c.OutputGate);


            System.Console.WriteLine("Input: " + 0);
            int output = c.Evaluate(0);

            System.Console.WriteLine("Output: " + output);
            System.Console.ReadKey();
        }
    }
}
