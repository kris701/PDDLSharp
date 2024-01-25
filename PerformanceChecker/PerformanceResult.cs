using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    internal class PerformanceResult
    {
        public string Name { get; }
        public int Iterations { get; }
        public int TotalFiles { get; set; }
        public long TotalSizeBytes { get; set; }
        public long TimeMs { get; private set; }
        private Stopwatch _watch = new Stopwatch();

        public PerformanceResult(string name, int iterations)
        {
            Name = name;
            Iterations = iterations;
            TotalFiles = 0;
            TotalSizeBytes = 0;
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
            Console.WriteLine($"\tSize:       {Math.Round((double)TotalSizeBytes / 1000000, 2)}MB");
            Console.WriteLine($"\tTime:       {Math.Round((double)TimeMs / 1000, 2)} s");
            Console.WriteLine($"\tThroughput: {Math.Round(((double)TotalSizeBytes / 1000000) / ((double)TimeMs / 1000), 2)}MB/s");
        }
    }
}
