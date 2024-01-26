using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    internal class TranslatorPerformanceResult : BaseTimedResult
    {
        public string Domain { get; }
        public int Problems { get; set; }
        public int Iterations { get; }
        public int TotalOperators { get; set; }
        public double OperatorS => Math.Round((double)TotalOperators / TimeS, 2);

        public TranslatorPerformanceResult(string domain, int iterations)
        {
            Domain = domain;
            Iterations = iterations;
        }
    }
}
