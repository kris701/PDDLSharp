namespace PerformanceChecker
{
    internal class FilePerformanceResult : BaseTimedResult
    {
        public string Name { get; }
        public int Iterations { get; }
        internal int TotalFiles { get; set; }
        internal long TotalSizeB { get; set; }
        public double TotalSizeMB => Math.Round((double)TotalSizeB / 1000000, 3);
        public double Throughput => Math.Round(TotalSizeMB / TimeS, 3);

        public FilePerformanceResult(string name, int iterations)
        {
            Name = name;
            Iterations = iterations;
        }
    }
}
