using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.FastDownward.SAS;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search.Classical;
using PDDLSharp.Tools;
using PDDLSharp.Translators;
using System.Text;
using ToMarkdown;
using ToMarkdown.Tables;

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
        private static int _iterations = 1;
        private static int _firstNProblems = 1;
        private static int _searchTimeLimit = 1;
#else
        private static int _iterations = 3;
        private static int _firstNProblems = 5;
        private static int _searchTimeLimit = 30;
#endif

        private static List<ThroughputResult> PDDL()
        {
            var result = new List<ThroughputResult>();
#if DEBUG
            var summary = BenchmarkRunner.Run<PDDLBenchmarks>(new DebugInProcessConfig());
#else
    var summary = BenchmarkRunner.Run<PDDLBenchmarks>();
#endif
            var targetHeader = summary.Table.Columns.First(x => x.Header.ToUpper() == "MEAN").Index;
            for(int i = 0; i < summary.Table.Columns[0].Content.Length; i++)
            {
                result.Add(new ThroughputResult(
                    summary.Table.Columns[0].Content[i],
                    TimeSpan.FromMicroseconds(Convert.ToDouble(summary.Table.Columns[targetHeader].Content[i].Replace("μs", ""))),
                    PDDLBenchmarks._domain.Length));
            }
            return result;
        }

        static async Task Main(string[] args)
        {
            //Console.WriteLine("Fetching Benchmarks");
            //var benchmarks = "../../../../Dependencies/downward-benchmarks";
            //if (!Directory.Exists(benchmarks))
            //    throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");
            //var benchmarkPlans = "../../../../Dependencies/PDDLBenchmarkPlans";
            //if (!Directory.Exists(benchmarkPlans))
            //    throw new DirectoryNotFoundException("Benchmarks not found! Please read the readme in the Dependencies folder!");

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
            sb.AppendLine(PDDL().ToMarkdownTable(
                new List<string>() { "*", "Time", "Size", "Throughput (MB/s)" }
                ));
            //sb.AppendLine();
            //sb.AppendLine("## Fast Downward SAS");
            //sb.AppendLine((await FastDownwardSAS(benchmarkPlans)).ToMarkdownTable(
            //    new List<string>() { "*", "*", "Total Size (MB)", "Throughput (MB/s)", "Total Time (s)" }
            //    ));
            //sb.AppendLine();
            //sb.AppendLine("## Fast Downward Plans");
            //sb.AppendLine((await FastDownwardPlans(benchmarkPlans)).ToMarkdownTable(
            //    new List<string>() { "*", "*", "Total Size (MB)", "Throughput (MB/s)", "Total Time (s)" }
            //    ));
            //sb.AppendLine();
            //sb.AppendLine("## Translation");
            //sb.AppendLine((await TranslatorPerformance(benchmarks)).ToMarkdownTable(
            //    new List<string>() { "*", "*", "*", "Total Operators", "Operators / second", "Total Time (s)" }
            //    ));
            //sb.AppendLine();

            //sb.AppendLine("# Toolkit Components");
            //sb.AppendLine($"## Planner (Classical, {_searchTimeLimit}s time limit)");
            //sb.AppendLine((await PlannerPerformance(benchmarks)).ToMarkdownTable(
            //    new List<string>() { "*", "*", "*", "*", "Generated / s", "Expansions / s", "Evaluations / s", "Solved (%)", "Total Time (s)" }
            //    ));
            //sb.AppendLine();

            var targetFile = "../../../readme.md";
            if (File.Exists(targetFile))
                File.Delete(targetFile);
            File.WriteAllText(targetFile, sb.ToString());
        }

        static async Task<List<FilePerformanceResult>> FastDownwardSAS(string benchmarkPlans)
        {
            var tasks = new List<Task<FilePerformanceResult>>();

            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("Fast Downward SAS Parsing", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var sasParser = new FDSASParser(errorListener);
                var domains = Directory.GetDirectories(Path.Combine(benchmarkPlans, "lama-first"));
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        int count = 0;
                        foreach (var file in Directory.GetFiles(domainPath))
                        {
                            if (new FileInfo(file).Extension == ".sas")
                            {
                                count++;
                                run.TotalSizeB += new FileInfo(file).Length * _iterations;
                                run.TotalFiles++;
                                var data = File.ReadAllText(file);
                                run.Start();
                                for (int i = 0; i < _iterations; i++)
                                    sasParser.Parse(data);
                                run.Stop();
                            }
                            if (count > _firstNProblems)
                                break;
                        }
                    }
                }
                Console.WriteLine($"{run.Name}\t Done!");
                return run;
            }));

            foreach (var task in tasks)
                task.Start();

            await Task.WhenAll(tasks);

            var results = new List<FilePerformanceResult>();
            foreach (var task in tasks)
                results.Add(task.Result);
            return results;
        }

        static async Task<List<FilePerformanceResult>> FastDownwardPlans(string benchmarkPlans)
        {
            var tasks = new List<Task<FilePerformanceResult>>();

            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("Fast Downward Plan Parsing", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var sasParser = new FDSASParser(errorListener);
                var domains = Directory.GetDirectories(Path.Combine(benchmarkPlans, "lama-first"));
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        int count = 0;
                        foreach (var file in Directory.GetFiles(domainPath))
                        {
                            if (new FileInfo(file).Extension == ".plan")
                            {
                                count++;
                                run.TotalSizeB += new FileInfo(file).Length * _iterations;
                                run.TotalFiles++;
                                var data = File.ReadAllText(file);
                                run.Start();
                                for (int i = 0; i < _iterations; i++)
                                    sasParser.Parse(data);
                                run.Stop();
                            }
                            if (count > _firstNProblems)
                                break;
                        }
                    }
                }
                Console.WriteLine($"{run.Name}\t Done!");
                return run;
            }));

            foreach (var task in tasks)
                task.Start();

            await Task.WhenAll(tasks);

            var results = new List<FilePerformanceResult>();
            foreach (var task in tasks)
                results.Add(task.Result);
            return results;
        }

        static async Task<List<TranslatorPerformanceResult>> TranslatorPerformance(string benchmarks)
        {
            var tasks = new List<Task<TranslatorPerformanceResult>>();

            var domains = Directory.GetDirectories(benchmarks);
            foreach (var domainPath in domains)
            {
                if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                {
                    var domainName = new DirectoryInfo(domainPath).Name;
                    var domainFile = Path.Combine(domainPath, "domain.pddl");
                    tasks.Add(new Task<TranslatorPerformanceResult>(() =>
                    {
                        var run = new TranslatorPerformanceResult(domainName, _iterations);
                        Console.WriteLine($"{run.Domain}\t Started...");
                        var errorListener = new ErrorListener(ParseErrorType.Error);
                        var pddlParser = new PDDLParser(errorListener);
                        var translator = new PDDLToSASTranslator(true);
                        var domain = pddlParser.ParseAs<DomainDecl>(new FileInfo(domainFile));
                        int count = 0;
                        foreach (var file in Directory.GetFiles(domainPath))
                        {
                            if (PDDLFileHelper.IsFileProblem(file))
                            {
                                count++;
                                run.Problems++;
                                var problem = pddlParser.ParseAs<ProblemDecl>(new FileInfo(file));
                                var decl = new PDDLDecl(domain, problem);
                                run.Start();
                                for (int i = 0; i < _iterations; i++)
                                {
                                    var sasDecl = translator.Translate(decl);
                                    run.TotalOperators += sasDecl.Operators.Count;
                                }
                                run.Stop();
                            }
                            if (count > _firstNProblems)
                                break;
                        }
                        Console.WriteLine($"{run.Domain}\t Done!");
                        return run;
                    }));
                }
            }

            foreach (var task in tasks)
                task.Start();

            await Task.WhenAll(tasks);

            var results = new List<TranslatorPerformanceResult>();
            foreach (var task in tasks)
            {
                var result = task.Result;
                results.Add(result);
            }
            return results;
        }

        static async Task<List<PlannerPerformanceResult>> PlannerPerformance(string benchmarks)
        {
            var tasks = new List<Task<PlannerPerformanceResult>>();

            var domains = Directory.GetDirectories(benchmarks);
            foreach (var domainPath in domains)
            {
                if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                {
                    var domainName = new DirectoryInfo(domainPath).Name;
                    var domainFile = Path.Combine(domainPath, "domain.pddl");

                    tasks.Add(new Task<PlannerPerformanceResult>(() =>
                    {
                        var run = new PlannerPerformanceResult(domainName, "Greedy Best First (hGoal)", _iterations);
                        Console.WriteLine($"{run.Domain}\t Started...");
                        var errorListener = new ErrorListener(ParseErrorType.Error);
                        var pddlParser = new PDDLParser(errorListener);
                        var translator = new PDDLToSASTranslator(true);
                        var domain = pddlParser.ParseAs<DomainDecl>(new FileInfo(domainFile));
                        int count = 0;
                        foreach (var file in Directory.GetFiles(domainPath))
                        {
                            if (PDDLFileHelper.IsFileProblem(file))
                            {
                                count++;
                                var problem = pddlParser.ParseAs<ProblemDecl>(new FileInfo(file));
                                var decl = new PDDLDecl(domain, problem);
                                var sasDecl = translator.Translate(decl);
                                run.Start();
                                for (int i = 0; i < _iterations; i++)
                                {
                                    run.Problems++;
                                    using (var planner = new GreedyBFS(sasDecl, new hGoal()))
                                    {
                                        planner.SearchLimit = TimeSpan.FromSeconds(_searchTimeLimit);
                                        var result = planner.Solve();
                                        if (!planner.Aborted)
                                            run.Solved++;
                                        run.Generated += planner.Generated;
                                        run.Expanded += planner.Expanded;
                                        run.Evaluations += planner.Evaluations;
                                    }
                                }
                                run.Stop();
                            }
                            if (count > _firstNProblems)
                                break;
                        }
                        Console.WriteLine($"{run.Domain}\t Done!");
                        return run;
                    }));
                    tasks.Add(new Task<PlannerPerformanceResult>(() =>
                    {
                        var run = new PlannerPerformanceResult(domainName, "Greedy Best First (hFF)", _iterations);
                        Console.WriteLine($"{run.Domain}\t Started...");
                        var errorListener = new ErrorListener(ParseErrorType.Error);
                        var pddlParser = new PDDLParser(errorListener);
                        var translator = new PDDLToSASTranslator(true);
                        var domain = pddlParser.ParseAs<DomainDecl>(new FileInfo(domainFile));
                        int count = 0;
                        foreach (var file in Directory.GetFiles(domainPath))
                        {
                            if (PDDLFileHelper.IsFileProblem(file))
                            {
                                count++;
                                var problem = pddlParser.ParseAs<ProblemDecl>(new FileInfo(file));
                                var decl = new PDDLDecl(domain, problem);
                                var sasDecl = translator.Translate(decl);
                                run.Start();
                                for (int i = 0; i < _iterations; i++)
                                {
                                    run.Problems++;
                                    using (var planner = new GreedyBFS(sasDecl, new hFF(sasDecl)))
                                    {
                                        planner.SearchLimit = TimeSpan.FromSeconds(_searchTimeLimit);
                                        var result = planner.Solve();
                                        if (!planner.Aborted)
                                            run.Solved++;
                                        run.Generated += planner.Generated;
                                        run.Expanded += planner.Expanded;
                                        run.Evaluations += planner.Evaluations;
                                    }
                                }
                                run.Stop();
                            }
                            if (count > _firstNProblems)
                                break;
                        }
                        Console.WriteLine($"{run.Domain}\t Done!");
                        return run;
                    }));
                }
            }

            foreach (var task in tasks)
                task.Start();

            await Task.WhenAll(tasks);

            var results = new List<PlannerPerformanceResult>();
            foreach (var task in tasks)
            {
                var result = task.Result;
                results.Add(result);
            }
            return results;
        }
    }
}