using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Circuits
{

    public abstract class GateConnection
    {
        public enum SideType
        {
            L,
            R,
            X
        }

        public readonly Gate Gate;
        public readonly SideType Side;

        public GateConnection(Gate gate, SideType side)
        {
            Gate = gate;
            Side = side;
        }

        public abstract int Value
        {
            get;
            set;
        }

        public void Reset()
        {
            Value = 0;
        }
    }

    public class GateInput : GateConnection
    {
        public GateInput(Gate gate, SideType side) 
            : base(gate, side)
        {
        }

        private GateOutput _input;
        public GateOutput Input
        {
            get { return _input; }
            set { ConnectTo(value); }
        }

        public void ConnectTo(GateOutput input)
        {
            if (input == _input)
                return;

            var temp = _input;
            _input = input;

            if (temp != null)
                temp.Output = null;
            
            if(_input != null)
                input.Output = this;
        }

        public override int Value
        {
            get { Debug.Assert(_input != null); return _input.Value; }
            set { Debug.Assert(_input != null); _input.Value = value; }
        }
    }

    public class GateOutput : GateConnection
    {
        public GateOutput(Gate gate, SideType side)
            : base(gate, side)
        {
        }

        private GateInput _output;
        public GateInput Output
        {
            get { return _output; }
            set { ConnectTo(value); }
        }

        public void ConnectTo(GateInput output)
        {
            if (_output == output)
                return;

            var temp = _output;
            _output = output;

            if (temp != null)
                temp.Input = null;
            
            if (_output != null)
                _output.Input = this;
        }

        private int _value;
        public override int Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }

    public class Gate
    {
        public readonly GateInput InputL;
        public readonly GateInput InputR;

        public readonly GateOutput OutputL;
        public readonly GateOutput OutputR;

        public readonly GateInput[] Inputs = new GateInput[2];
        public readonly GateOutput[] Outputs = new GateOutput[2];

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

            Inputs[0] = InputL;
            Inputs[1] = InputR;
            Outputs[0] = OutputL;
            Outputs[1] = OutputR;
        }

        public override string ToString()
        {
            return "Gate[" + Index + "]";
        }

        public virtual void Evaluate()
        {
            var input = Tuple.Create(InputL.Value, InputR.Value);
            Tuple<int, int> output;

            if (LookupTable.TryGetValue(input, out output))
            {
                //Debug.WriteLine(this + ":" + input + " returned " + output);
                OutputL.Value = output.Item1;
                OutputR.Value = output.Item2;
            }
            else // dunno what this input combination does
            {
                // can't get here
                Debug.Assert(false, "Unknown Gate Input: " + input);
            }
        }

        public void Reset()
        {
            OutputL.Value = 0;
            OutputR.Value = 0;
        }

        // thank you visio
        static Dictionary<Tuple<int, int>, Tuple<int, int>> LookupTable = new Dictionary<Tuple<int, int>, Tuple<int, int>> { 
            { Tuple.Create(0, 0), Tuple.Create(0, 2) },
            { Tuple.Create(1, 0), Tuple.Create(1, 2) },
            { Tuple.Create(2, 0), Tuple.Create(2, 2) },
            { Tuple.Create(0, 1), Tuple.Create(2, 2) },
            { Tuple.Create(1, 1), Tuple.Create(0, 0) },
            { Tuple.Create(2, 1), Tuple.Create(1, 1) },
            { Tuple.Create(0, 2), Tuple.Create(1, 2) },
            { Tuple.Create(1, 2), Tuple.Create(2, 1) },
            { Tuple.Create(2, 2), Tuple.Create(0, 0) },
        };
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

    public class Circuit
    {
        public List<Gate> Gates = new List<Gate>();        

        public GateOutput InputStream;
        public GateInput OutputStream;
        public ExternalGate ExternalGate;

        public int Size
        {
            get { return Gates.Count; }
        }

        public Circuit()
        {            
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
            InputStream.Value = input;

            foreach (Gate g in Gates)
                g.Evaluate();
           
            return OutputStream.Value;
        }

        public string Evaluate(string input)
        {
            return Evaluate(input.ToStream()).FromStream();
        }

        public int[] Evaluate(int[] inputstream)
        {
            int[] outputstream = new int[inputstream.Length];

            for (int i = 0; i < inputstream.Length; i++)
                outputstream[i] = Evaluate(inputstream[i]);

            Reset();

            return outputstream;
        }

        public void Reset()
        {
            foreach (Gate g in Gates)
                g.Reset();
        }
    }
}
