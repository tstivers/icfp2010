using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Circuits
{
    public static class CircuitUtility
    {
        public static int[] ToStream(this string stream)
        {
            int[] bleh = new int[stream.Trim().Length];

            int index = 0;
            foreach (char c in stream.Trim())
                bleh[index++] = int.Parse("" + c);

            return bleh;
        }

        public static string FromStream(this int[] stream)
        {
            string output = "";
            foreach(int i in stream)
                output += i.ToString();

            return output;
        }

        public static string DumpCircuit(this Circuit c)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(c.InputStream.Wire.End.Gate.Index);
            sb.Append(c.InputStream.Wire.End.Side);
            sb.Append(":\n");
            for(int i = 0; i < c.Gates.Count; i++){
                if (!c.Gates[i].InputL.Wire.Start.Gate.IsExternal)
                    sb.Append(c.Gates[i].InputL.Wire.Start.Gate.Index);

                sb.Append(c.Gates[i].InputL.Wire.Start.Side);

                if (!c.Gates[i].InputR.Wire.Start.Gate.IsExternal)
                    sb.Append(c.Gates[i].InputR.Wire.Start.Gate.Index);

                sb.Append(c.Gates[i].InputR.Wire.Start.Side);
                sb.Append("0#");
                if (!c.Gates[i].OutputL.Wire.End.Gate.IsExternal)
                    sb.Append(c.Gates[i].OutputL.Wire.End.Gate.Index);

                sb.Append(c.Gates[i].OutputL.Wire.End.Side);
                if (!c.Gates[i].OutputR.Wire.End.Gate.IsExternal)
                    sb.Append(c.Gates[i].OutputR.Wire.End.Gate.Index);

                sb.Append(c.Gates[i].OutputR.Wire.End.Side);
                sb.Append((i + 1) == c.Gates.Count ? ":\n" : ",\n");
            }

            sb.Append(c.OutputStream.Wire.Start.Gate.Index);
            sb.Append(c.OutputStream.Wire.Start.Side);

            return sb.ToString();
        }

        public static Circuit BuildCircuit(string inputText)
        {
            Regex regexCapture = new Regex(@"(\d+)?(\w)(\d+)?(\w)0#(\d+)?(\w)(\d+)?(\w)");
            
            var rawList = inputText.Split('\n').ToList<string>();
            
            // filter blank lines and comments
            var gateList = rawList.Where(x => x.Trim().Length > 0 && !x.StartsWith("#")).ToList();

            // remove inputsteam/outputstream lines
            gateList.RemoveAt(0);
            gateList.RemoveAt(gateList.Count - 1);

            Circuit c = new Circuit();

            for (int i = 0; i < gateList.Count; i++)
            {
                var gate = c.AddGate(i);
            }

            for(int i = 0; i < gateList.Count; i++)
            { 
                MatchCollection mc = regexCapture.Matches(gateList[i]);
                if (mc[0].Groups[2].Value == "X")
                    c.Gates[i].InputL.ConnectTo(c.InputStream);
                else if (mc[0].Groups[2].Value == "L")
                    c.Gates[i].InputL.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[1].Value)].OutputL);
                else if (mc[0].Groups[2].Value == "R")
                    c.Gates[i].InputL.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[1].Value)].OutputR);

                if (mc[0].Groups[4].Value == "X")
                    c.Gates[i].InputR.ConnectTo(c.InputStream);
                else if (mc[0].Groups[4].Value == "L")
                    c.Gates[i].InputR.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[3].Value)].OutputL);
                else if (mc[0].Groups[4].Value == "R")
                    c.Gates[i].InputR.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[3].Value)].OutputR);

                if (mc[0].Groups[6].Value == "X")
                    c.Gates[i].OutputL.ConnectTo(c.OutputStream);
                else if (mc[0].Groups[6].Value == "L")
                    c.Gates[i].OutputL.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[5].Value)].InputL);
                else if (mc[0].Groups[6].Value == "R")
                    c.Gates[i].OutputL.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[5].Value)].InputR);

                if (mc[0].Groups[8].Value == "X")
                    c.Gates[i].OutputR.ConnectTo(c.OutputStream);
                else if (mc[0].Groups[8].Value == "L")
                    c.Gates[i].OutputR.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[7].Value)].InputL);
                else if (mc[0].Groups[8].Value == "R")
                    c.Gates[i].OutputR.ConnectTo(c.Gates[Int32.Parse(mc[0].Groups[7].Value)].InputR);
            }

            return c;
        }
    }
}
