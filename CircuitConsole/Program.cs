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
            Circuit circuit = new Circuit();
            Gate gate0 = circuit.AddGate(0);
            Gate gate1 = circuit.AddGate(1);
            gate0.InputL = circuit.InputWire;
            gate0.InputR.ConnectTo()
        }
    }
}
