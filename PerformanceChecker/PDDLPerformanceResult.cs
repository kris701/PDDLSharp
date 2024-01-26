using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    internal class PDDLPerformanceResult
    {
        public string Name { get; }
        public int Iterations { get; }
        internal int TotalFiles { get; set; }
        internal long TotalSizeB { get; set; }
        public double TotalSizeMB => Math.Round((double)TotalSizeB / 1000000, 2);
        internal long TimeMs { get; private set; }
        public double TimeS => Math.Round((double)TimeMs / 1000, 2);
        public double Throughput => Math.Round(TotalSizeMB / TimeS, 2);
        private Stopwatch _watch = new Stopwatch();

        public PDDLPerformanceResult(string name, int iterations)
        {
            Name = name;
            Iterations = iterations;
            TotalFiles = 0;
            TotalSizeB = 0;
            TimeMs = 0;
        }

        public void Start()
        {
            _watch.Start();
        }

        public void Stop()
        {
            _watch.Stop();
            TimeMs = _watch.ElapsedMilliseconds;
        }

        public void Report()
        {
            Console.WriteLine($"\tName:       {Name}");
            Console.WriteLine($"\tFiles:      {TotalFiles}");
            Console.WriteLine($"\tIterations: {Iterations}");
            Console.WriteLine($"\tSize:       {TotalSizeMB}MB");
            Console.WriteLine($"\tTime:       {TimeS} s");
            Console.WriteLine($"\tThroughput: {Throughput}MB/s");
        }
    }
}
