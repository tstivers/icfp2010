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

        public int Index
        {
            get { return Side == SideType.L ? 0 : 1; }
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

        public GateOutput Source;
       
        public void ConnectTo(GateOutput source)
        {
            if (source == Source)
                return;

            if (source != null && source.Target != null)
                source.Target.Source = null;

            if (Source != null)
                Source.Target = null;

            Source = source;

            if (Source != null)
                Source.Target = this;
        }

        public override int Value
        {
            get { Debug.Assert(Source != null); return Source.Value; }
            set { Debug.Assert(Source != null); Source.Value = value; }
        }

        public override string ToString()
        {
            if (Source == null)
                return "???";
            return Source.Gate.IsExternal ? Source.Side.ToString() : Source.Gate.Index.ToString() + Source.Side;
        }
    }

    public class GateOutput : GateConnection
    {
        public GateOutput(Gate gate, SideType side)
            : base(gate, side)
        {            
        }
        
        public GateInput Target;        

        public void ConnectTo(GateInput target)
        {
            if (Target == target)
                return;

            if (target != null && target.Source != null)
                target.Source.Target = null;

            if (Target != null)
                Target.Source = null;

            Target = target;

            if (target != null)
                target.Source = this;
        }

        private int _value;
        public override int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override string ToString()
        {
            if (Target == null)
                return "???";
            return Target.Gate.IsExternal ? Target.Side.ToString() : Target.Gate.Index.ToString() + Target.Side;
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
            get { return _index; }
        }       

        public virtual bool IsExternal
        {
            get { return false; }
        }

        public readonly Circuit Circuit;

        private readonly int _index;

        public Gate(Circuit circuit, int index)
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
            _index = index;
        }

        public override string ToString()
        {
            return InputL.ToString() + InputR + "0#" + OutputL + OutputR;
        }

        public virtual void Evaluate()
        {
            int[] output = table[InputL.Value, InputR.Value];
            OutputL.Value = output[0];
            OutputR.Value = output[1];
        }

        public void Reset()
        {
            OutputL.Value = 0;
            OutputR.Value = 0;
        }

        static int[,][] table = new int[3,3][] {
            {new int[]{0,2}, new int[]{2,2}, new int[]{1,2}},
            {new int[]{1,2}, new int[]{0,0}, new int[]{2,1}},
            {new int[]{2,2}, new int[]{1,1}, new int[]{0,0}}};       
    }

    public class ExternalGate : Gate
    {
        public ExternalGate(Circuit circuit)
            : base(circuit, -1)
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
        public Gate[] Gates;
        public List<GateOutput> Outputs = new List<GateOutput>();
        public List<GateInput> Inputs = new List<GateInput>();

        public GateOutput InputStream;
        public GateInput OutputStream;
        public ExternalGate ExternalGate;

        public int Size
        {
            get { return Gates.Length; }
        }

        public Circuit(int gateCount)
        {
            Gates = new Gate[gateCount];
            ExternalGate = new ExternalGate(this);
            InputStream = new GateOutput(ExternalGate, GateConnection.SideType.X);
            OutputStream = new GateInput(ExternalGate, GateConnection.SideType.X);
        }

        public Gate AddGate()
        {
            var gate = new Gate(this, Gates.Count(x => x != null));
            Gates[Gates.Count(x => x != null)] = gate;
            Inputs.Add(gate.InputL);
            Inputs.Add(gate.InputR);
            Outputs.Add(gate.OutputL);
            Outputs.Add(gate.OutputR);
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

        public bool Evaluate(int[] inputstream, int[] outputstream)
        {
            Reset();
            //System.Console.WriteLine("Evaluating:\n\n" + this);
            //System.Console.WriteLine("Output: " + Evaluate(inputstream).FromStream());
            for(int i = 0; i < inputstream.Length; i++)
                if((outputstream[i] != Evaluate(inputstream[i])) && (outputstream[i] != 4))
                    return false;           

            return true;
        }

        public void Reset()
        {
            foreach (Gate g in Gates)
                g.Reset();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(InputStream.ToString());
            sb.Append(":\n");
            foreach (Gate g in Gates.Take(Gates.Length - 1))
                sb.Append(" " + g.ToString() + ",\n");
            sb.Append(" " + Gates[Gates.Length - 1].ToString() + ":\n");
            sb.Append(OutputStream + "\n");
            return sb.ToString();
        }
    }
}
