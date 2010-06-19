using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Circuits
{
    class CircuitUtility
    {
        public static string DumpCircuit(Circuit c)
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
    }
}
