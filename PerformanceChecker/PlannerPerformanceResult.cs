namespace PerformanceChecker
{
    internal class PlannerPerformanceResult : BaseTimedResult
    {
        public string Domain { get; }
        public string Planner { get; }
        public int Problems { get; set; }
        public int Iterations { get; }
        internal long Generated { get; set; }
        public double GeneratedS => Math.Round((double)Generated / TimeS, 3);
        internal long Expanded { get; set; }
        public double ExpandedS => Math.Round((double)Expanded / TimeS, 3);
        internal long Evaluations { get; set; }
        public double EvaluationsS => Math.Round((double)Evaluations / TimeS, 3);
        internal int Solved { get; set; }
        public double SolvePercent => Math.Round(((double)Solved / Problems) * 100, 3);

        public PlannerPerformanceResult(string domain, string planner, int iterations)
        {
            Domain = domain;
            Planner = planner;
            Iterations = iterations;
        }
    }
}
