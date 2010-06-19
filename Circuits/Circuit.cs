using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Circuits
{

    public class GateConnection
    {
        public enum SideType
        {
            L,
            R,
            X
        }

        public Wire Wire;
        public readonly Gate Gate;
        public readonly SideType Side;

        public GateConnection(Gate gate, SideType side)
        {
            Gate = gate;
            Side = side;
        }
    }

    public class GateInput : GateConnection
    {
        public GateInput(Gate gate, SideType side) 
            : base(gate, side)
        {
        }

        public void ConnectTo(GateOutput output)
        {
            Wire = new Wire(output, this);
            output.Wire = Wire;
        }
    }

    public class GateOutput : GateConnection
    {
        public GateOutput(Gate gate, SideType side)
            : base(gate, side)
        {
        }

        public void ConnectTo(GateInput input)
        {
            Wire = new Wire(this, input);
            input.Wire = Wire;
        }
    }

    public class Gate
    {
        public readonly GateInput InputL;
        public readonly GateInput InputR;

        public readonly GateOutput OutputL;
        public readonly GateOutput OutputR;

        public GateInput GetInput(GateConnection.SideType side)
        {
            return side == GateConnection.SideType.L ? InputL : InputR;
        }

        public GateOutput GetOutput(GateConnection.SideType side)
        {
            return side == GateConnection.SideType.L ? OutputL : OutputR;
        }

        public virtual int Index
        {
            get { return Circuit.Gates.IndexOf(this); }
        }

        // property, compare _evaluatedIndex to circuit->CurrentIndex
        public bool Evaluated;

        public virtual bool IsExternal
        {
            get { return false; }
        }

        public readonly Circuit Circuit;

        public Gate(Circuit circuit)
        {
            Circuit = circuit;
            InputL = new GateInput(this, GateConnection.SideType.L);
            InputR = new GateInput(this, GateConnection.SideType.R);

            OutputL = new GateOutput(this, GateConnection.SideType.L);
            OutputR = new GateOutput(this, GateConnection.SideType.R);
        }

        public override string ToString()
        {
            return "Gate[" + Index + "]";
        }

        public virtual void Evaluate()
        {
            if (Evaluated)
                return;

            Evaluated = true;
            
            var input = Tuple.Create(InputL.Wire.Value, InputR.Wire.Value);
            Tuple<int, int> output;

            if (LookupTable.TryGetValue(input, out output))
            {
                System.Console.WriteLine(this + ":" + input + " returned " + output);

                OutputL.Wire.Value = output.Item1;
                OutputL.Wire.End.Gate.Evaluate();

                OutputR.Wire.Value = output.Item2;
                OutputR.Wire.End.Gate.Evaluate();
            }
            else // dunno what this input combination does
            {
                System.Console.WriteLine(this + ": unknown input " + input);
            }
        }

        static Dictionary<Tuple<int, int>, Tuple<int, int>> LookupTable = new Dictionary<Tuple<int, int>, Tuple<int, int>> { { Tuple.Create(0, 0), Tuple.Create(0, 2) } };
    }

    public class ExternalGate : Gate
    {
        public ExternalGate(Circuit circuit)
            : base(circuit)
        {
        }

        public override void Evaluate()
        {
        }

        public override int Index
        {
            get { return -1; }
        }

        public override bool IsExternal
        {
            get { return true; }
        }
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

        public GateOutput InputStream;
        public GateInput OutputStream;
        public ExternalGate ExternalGate;

        public int Size
        {
            get { return Gates.Count; }
        }

        public Circuit()
        {
            Gates = new List<Gate>();
            ExternalGate = new ExternalGate(this);
            InputStream = new GateOutput(ExternalGate, GateConnection.SideType.X);
            OutputStream = new GateInput(ExternalGate, GateConnection.SideType.X);
        }

        public Gate AddGate(int index)
        {
            var gate = new Gate(this);
            Gates.Insert(index, gate);
            return gate;
        }

        public int Evaluate(int input)
        {
            InputStream.Wire.Value = input;
            InputStream.Wire.End.Gate.Evaluate();

            // reset the evaluation flag for the next step
            foreach (Gate g in Gates)
                g.Evaluated = false;

            return OutputStream.Wire.Value;
        }
    }
}
