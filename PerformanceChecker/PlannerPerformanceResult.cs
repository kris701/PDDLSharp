using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceChecker
{
    internal class PlannerPerformanceResult : BaseTimedResult
    {
        public string Domain { get; }
        public string Planner { get; }
        public int Problems { get; set; }
        public int Iterations { get; }
        internal long Generated { get; set; }
        public double GeneratedS => Math.Round((double)Generated / TimeS, 2);
        internal long Expanded { get; set; }
        public double ExpandedS => Math.Round((double)Expanded / TimeS, 2);
        internal long Evaluations { get; set; }
        public double EvaluationsS => Math.Round((double)Evaluations / TimeS, 2);
        internal int Solved { get; set; }
        public double SolvePercent => Math.Round(((double)Solved / Problems) * 100, 2);

        public PlannerPerformanceResult(string domain, string planner, int iterations)
        {
            Domain = domain;
            Planner = planner;
            Iterations = iterations;
        }
    }
}
