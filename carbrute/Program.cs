using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace carbrute
{
    class Program
    {
        static void Main(string[] args)
        {
            int start = 285;
            string preamble = "12222";
            string postamble = "22000";
            int startzeros = 4;

            using (var bleh = new StreamWriter("cars.txt"))
            {
                for (int i = 0; i < 70; i++)
                {
                    string output = preamble + ternary(i + start, 6);
                    for (int j = 0; j < startzeros + i; j++)
                        output += "0";
                    output += postamble;

                    bleh.WriteLine(output);
                }
            }
        }

        static public string ternary(int num, int digits)
        {
            string bleh = "";
            for (int i = 1; i <= digits; i++)
            {
                bleh = num % 3 + bleh;
                num /= 3;
            }
            return bleh;
        }
    }


}
