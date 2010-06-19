using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Circuits
{

    public class GateConnection
    {
        public Wire Wire;
    }

    public class GateInput : GateConnection
    {
    }

    public class GateOutput : GateConnection
    {
    }

    public class Gate
    {
        public GateInput InputL;
        public GateInput InputR;

        public GateOutput OutputL;
        public GateOutput OutputR;

        public int Index
        {
            get { return Circuit.Gates.IndexOf(this); }
        }

        // property, compare _evaluatedIndex to circuit->CurrentIndex
        public bool Evaluated;

        public readonly Circuit Circuit;

        public Gate(Circuit circuit)
        {
            Circuit = circuit;
            InputL = new GateInput();
            InputR = new GateInput();

            OutputL = new GateOutput();
            OutputR = new GateOutput();
        }

        public void Evaluate()
        {
            if (Evaluated)
                return;

            var found = LookupTable[Tuple.Create(InputL.Wire.Value, InputR.Wire.Value)];

            if (found != null)
            {
                OutputL.Wire.Value = found.Item1;
                OutputL.Wire.End.Evaluate();

                OutputR.Wire.Value = found.Item2;
                OutputR.Wire.End.Evaluate();
            }
            else // dunno what this input combination does
            {
            }

            // no
            Evaluated = true;
        }

        static Dictionary<Tuple<int, int>, Tuple<int, int>> LookupTable = new Dictionary<Tuple<int, int>, Tuple<int, int>> { { Tuple.Create(0, 0), Tuple.Create(0, 2) } };
    }

    public class Wire
    {
        public Gate Start;
        public Gate End;

        private int _currentValue;
        public int Value
        {
            get { return _currentValue; }
            set { _currentValue = value; }
        }
    }

    public class Circuit
    {
        public List<Gate> Gates;

        public Wire InputWire;
        public Wire OutputWire;

        
        public int Size
        {
            get { return Gates.Count; }
        }

        public Circuit()
        {
            Gates = new List<Gate>();
            InputWire = new Wire();
            OutputWire = new Wire();
        }

        public Gate AddGate(int index)
        {
            return new Gate(this);
        }

        private byte[] _input;
        public string Input
        {
            get { return arrayToString(_input); }
            set { _input = arrayFromString(value); }
        }

        private byte[] _output;
        public string Output
        {
            get { return arrayToString(_output); }
            set { _output = arrayFromString(value); }
        }

        public string Text
        {
            get { return this.ToString(); }
            set { this.ParseCircuit(value); }
        }

        public void ParseCircuit(string text)
        {
        }

        override public string ToString()
        {
            return "bleh";
        }

        private string arrayToString(byte[] array)
        {
            return "bleh";
        }

        private byte[] arrayFromString(string array)
        {
            return new byte[100];
        }
    }
}
