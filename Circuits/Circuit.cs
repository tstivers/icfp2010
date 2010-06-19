using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Circuits
{

    public class GateConnection
    {
        public Wire Wire;
        public readonly Gate Gate;

        public GateConnection(Gate gate)
        {
            Gate = gate;
        }
    }

    public class GateInput : GateConnection
    {
        public GateInput(Gate gate) 
            : base(gate)
        {
        }

        public void ConnectTo(GateOutput output)
        {
            Wire = new Wire(output, this);
        }
    }

    public class GateOutput : GateConnection
    {
        public GateOutput(Gate gate)
            : base(gate)
        {
        }

        public void ConnectTo(GateInput input)
        {
            Wire = new Wire(this, input);
        }

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
            InputL = new GateInput(this);
            InputR = new GateInput(this);

            OutputL = new GateOutput(this);
            OutputR = new GateOutput(this);
        }

        public void Evaluate()
        {
            if (Evaluated)
                return;

            var found = LookupTable[Tuple.Create(InputL.Wire.Value, InputR.Wire.Value)];

            if (found != null)
            {
                OutputL.Wire.Value = found.Item1;
                OutputL.Wire.End.Gate.Evaluate();

                OutputR.Wire.Value = found.Item2;
                OutputR.Wire.End.Gate.Evaluate();
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
        public readonly GateOutput Start;
        public readonly GateInput End;

        private int _currentValue;
        public int Value
        {
            get { return _currentValue; }
            set { _currentValue = value; }
        }

        public Wire(GateOutput start, GateInput end)
        {
            Start = start;
            End = end;
        }
    }

    public class Circuit
    {
        public List<Gate> Gates;

        public GateOutput InputGate;
        public GateInput OutputGate;
        public Gate ExternalGate;

        
        public int Size
        {
            get { return Gates.Count; }
        }

        public Circuit()
        {
            Gates = new List<Gate>();
            ExternalGate = new Gate(this);
            InputGate = new GateOutput(ExternalGate);
            OutputGate = new GateInput(ExternalGate);
        }

        public Gate AddGate(int index)
        {
            return new Gate(this);
        }

        public int Evaluate(int input)
        {
            InputGate.Wire.Value = input;
            InputGate.Wire.End.Gate.Evaluate();

            return OutputGate.Wire.Value;
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
