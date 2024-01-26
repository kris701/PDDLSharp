using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    internal class FilePerformanceResult : BaseTimedResult
    {
        public string Name { get; }
        public int Iterations { get; }
        internal int TotalFiles { get; set; }
        internal long TotalSizeB { get; set; }
        public double TotalSizeMB => Math.Round((double)TotalSizeB / 1000000, 2);
        public double Throughput => Math.Round(TotalSizeMB / TimeS, 2);

        public FilePerformanceResult(string name, int iterations)
        {
            Name = name;
            Iterations = iterations;
        }
    }
}
