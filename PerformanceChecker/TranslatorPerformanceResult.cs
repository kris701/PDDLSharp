namespace PerformanceChecker
{
    internal class TranslatorPerformanceResult : BaseTimedResult
    {
        public string Domain { get; }
        public int Problems { get; set; }
        public int Iterations { get; }
        public int TotalOperators { get; set; }
        public double OperatorS => Math.Round((double)TotalOperators / TimeS, 3);

        public TranslatorPerformanceResult(string domain, int iterations)
        {
            Domain = domain;
            Iterations = iterations;
        }
    }
}
