using System;
using System.Collections.Generic;
using System.Text;

namespace KT_41
{
    internal class Result
    {
        public string Name;
        public double TimeMs;
        public int Gen0;
        public int Gen1;
        public int Gen2;

        public Result(string name, double timeMs, int gen0, int gen1, int gen2)
        {
            Name = name;
            TimeMs = timeMs;
            Gen0 = gen0;
            Gen1 = gen1;
            Gen2 = gen2;
        }
    }
}