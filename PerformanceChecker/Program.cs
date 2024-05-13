using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System.Text;
using ToMarkdown;

namespace PerformanceChecker
{
    internal class Program
    {
        public static string _header = "# PDDLSharp Performance\r\nHere are some general statistics about how good the PDDLSharp system performs.\r\n\r\n";

        public static List<string> TargetDomains = new List<string>()
        {
            "gripper",
            "blocks",
            "depot",
            "miconic",
            "rovers",
            "trucks",
            "zenotravel"
        };
#if DEBUG
        private static readonly int _iterations = 1;
        private static readonly int _firstNProblems = 1;
        private static readonly int _searchTimeLimit = 1;
#else
        private static int _iterations = 3;
        private static int _firstNProblems = 5;
        private static int _searchTimeLimit = 30;
#endif

        private static List<ThroughputResult> PDDLBenchmarks()
        {
            var result = new List<ThroughputResult>();
#if DEBUG
            var summary = BenchmarkRunner.Run<PDDLBenchmarks>(new DebugInProcessConfig());
#else
    var summary = BenchmarkRunner.Run<PDDLBenchmarks>();
#endif
            var targetHeader = summary.Table.Columns.First(x => x.Header.ToUpper() == "MEAN").Index;
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[0],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[0].Replace("μs", ""))),
                PerformanceChecker.PDDLBenchmarks._domain.Length));
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[1],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[1].Replace("μs", ""))),
                PerformanceChecker.PDDLBenchmarks._problem.Length));
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[2],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[2].Replace("μs", ""))),
                PerformanceChecker.PDDLBenchmarks._domain.Length + PerformanceChecker.PDDLBenchmarks._problem.Length));
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[3],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[3].Replace("μs", ""))),
                PerformanceChecker.PDDLBenchmarks._domain.Length + PerformanceChecker.PDDLBenchmarks._problem.Length));
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[4],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[4].Replace("μs", ""))),
                PerformanceChecker.PDDLBenchmarks._domain.Length));
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[5],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[5].Replace("μs", ""))),
                PerformanceChecker.PDDLBenchmarks._problem.Length));
            return result;
        }

        private static List<ThroughputResult> FDBenchmarks()
        {
            var result = new List<ThroughputResult>();
#if DEBUG
            var summary = BenchmarkRunner.Run<FDBenchmarks>(new DebugInProcessConfig());
#else
            var summary = BenchmarkRunner.Run<FDBenchmarks>();
#endif
            var targetHeader = summary.Table.Columns.First(x => x.Header.ToUpper() == "MEAN").Index;
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[0],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[0].Replace("μs", ""))),
                PerformanceChecker.FDBenchmarks._sas.Length));
            result.Add(new ThroughputResult(
                summary.Table.Columns[0].Content[1],
                TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[1].Replace("μs", ""))),
                PerformanceChecker.FDBenchmarks._plan.Length));
            return result;
        }

        static async Task Main(string[] args)
        {
            var sb = new StringBuilder();
#if DEBUG
            sb.AppendLine("# RESULTS ARE IN DEBUG MODE");
#endif
            sb.AppendLine(_header);
            sb.AppendLine("These benchmarks are based on a single gripper domain");
            sb.Append("BenchmarkDotNet".ToMarkdownLink("https://github.com/dotnet/BenchmarkDotNet"));
            sb.AppendLine(" is used to generated the time results.");
            sb.AppendLine("# Core Components");
            sb.AppendLine("## PDDL");
            sb.AppendLine(PDDLBenchmarks().ToMarkdownTable(
                new List<string>() { "*", "Time", "Size", "Throughput (MB/s)" }
                ));
            sb.AppendLine();
            sb.AppendLine("## Fast Downward");
            sb.AppendLine(FDBenchmarks().ToMarkdownTable(
                new List<string>() { "*", "Time", "Size", "Throughput (MB/s)" }
                ));
            sb.AppendLine();

            var targetFile = "../../../readme.md";
            if (File.Exists(targetFile))
                File.Delete(targetFile);
            File.WriteAllText(targetFile, sb.ToString());
        }
    }
}