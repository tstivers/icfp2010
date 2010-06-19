using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Circuits
{
    public class Gate
    {
        public Wire InputL;
        public Wire InputR;

        public Wire OutputL;
        public Wire OutputR;

        public int Index
        {
            get { return Circuit.Gates.IndexOf(this); }
        }

        public Circuit Circuit;
    }

    public class Wire
    {
        public Gate Start;
        public Gate End;

        private byte _currentValue;
        private byte _nextValue;
        public byte Value
        {
            get { return _currentValue; }
            set
            {
                if (Start.Index <= End.Index)
                    _currentValue = value;
                else
                {
                    _currentValue = _nextValue;
                    _nextValue = value;
                }
            }
        }

        public void Reset()
        {
            _currentValue = 0;
            _nextValue = 0;
        }
    }

    public class Circuit
    {
        public List<Gate> Gates;

        public int Size
        {
            get { return Gates.Count; }
        }

        public Circuit()
        {
            Gates = new List<Gate>();
        }

        public Gate AddGate(int index)
        {
            return new Gate();
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
