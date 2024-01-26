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
using System;
using System.Diagnostics;
using System.Text;
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

        static async Task Main(string[] args)
        {
            Console.WriteLine("Fetching Benchmarks");
            var benchmarks = await GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/aibasel/downward-benchmarks/", "benchmarks");
            var benchmarkPlans = await GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/kris701/PDDLBenchmarkPlans", "benchmarks-plans");

            var sb = new StringBuilder();
#if DEBUG
            sb.AppendLine("# RESULTS ARE IN DEBUG MODE");
#endif
            sb.AppendLine(_header);
            sb.AppendLine("These benchmarks made on the following domains from [Fast Downward](https://github.com/aibasel/downward-benchmarks/):");
            sb.AppendLine(TargetDomains.ToMarkdownList());
            sb.AppendLine($"For each of these domains, the first {_firstNProblems} problems are selected.");
            sb.AppendLine($"Each component is executed {_iterations} times to get a better average.");
            sb.AppendLine("# Core Components");
            sb.AppendLine("## PDDL");
            sb.AppendLine((await PDDLPerformance(benchmarks)).ToMarkdownTable(
                new List<string>() { "*", "*", "Total Size (MB)", "Throughput (MB/s)", "Total Time (s)" }
                ));
            sb.AppendLine();
            sb.AppendLine("## Fast Downward SAS");
            sb.AppendLine((await FastDownwardSAS(benchmarkPlans)).ToMarkdownTable(
                new List<string>() { "*", "*", "Total Size (MB)", "Throughput (MB/s)", "Total Time (s)" }
                ));
            sb.AppendLine();
            sb.AppendLine("## Fast Downward Plans");
            sb.AppendLine((await FastDownwardPlans(benchmarkPlans)).ToMarkdownTable(
                new List<string>() { "*", "*", "Total Size (MB)", "Throughput (MB/s)", "Total Time (s)" }
                ));
            sb.AppendLine();
            sb.AppendLine("## Translation");
            sb.AppendLine((await TranslatorPerformance(benchmarks)).ToMarkdownTable(
                new List<string>() { "*", "*", "*", "Total Operators", "Operators / second", "Total Time (s)" }
                ));
            sb.AppendLine();

            sb.AppendLine("# Toolkit Components");
            sb.AppendLine($"## Planner (Classical, {_searchTimeLimit}s time limit)");
            sb.AppendLine((await PlannerPerformance(benchmarks)).ToMarkdownTable(
                new List<string>() { "*", "*", "*", "*", "Generated / s", "Expansions / s", "Evaluations / s", "Solved (%)", "Total Time (s)" }
                ));
            sb.AppendLine();

            var targetFile = "../../../readme.md";
            if (File.Exists(targetFile))
                File.Delete(targetFile);
            File.WriteAllText(targetFile, sb.ToString());
        }   
        
        static async Task<List<FilePerformanceResult>> PDDLPerformance(string benchmarks)
        {
            var tasks = new List<Task<FilePerformanceResult>>();

            Console.WriteLine("PDDL Domain Parsing");
            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("PDDL Domain Parsing", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var pddlParser = new PDDLParser(errorListener);
                var domains = Directory.GetDirectories(benchmarks);
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        var domainFile = Path.Combine(domainPath, "domain.pddl");
                        if (File.Exists(domainFile))
                        {
                            run.TotalSizeB += new FileInfo(domainFile).Length * _iterations;
                            run.TotalFiles++;
                            var data = File.ReadAllText(domainFile);
                            run.Start();
                            for (int i = 0; i < _iterations; i++)
                                pddlParser.ParseAs<DomainDecl>(data);
                            run.Stop();
                        }
                    }
                }
                Console.WriteLine($"{run.Name}\t Done!");
                return run;
            }));
            Console.WriteLine("PDDL Problem Parsing");
            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("PDDL Problem Parsing", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var pddlParser = new PDDLParser(errorListener);
                var domains = Directory.GetDirectories(benchmarks);
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        int count = 0;
                        foreach (var file in Directory.GetFiles(domainPath))
                        {
                            if (PDDLFileHelper.IsFileProblem(file))
                            {
                                count++;
                                run.TotalSizeB += new FileInfo(file).Length * _iterations;
                                run.TotalFiles++;
                                var data = File.ReadAllText(file);
                                run.Start();
                                for (int i = 0; i < _iterations; i++)
                                    pddlParser.ParseAs<ProblemDecl>(data);
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
            Console.WriteLine("PDDL Contextualization");
            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("PDDL Contextualization", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var pddlParser = new PDDLParser(errorListener);
                var contextualizer = new PDDLContextualiser(errorListener);
                var domains = Directory.GetDirectories(benchmarks);
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        var domainFile = Path.Combine(domainPath, "domain.pddl");
                        int count = 0;
                        if (File.Exists(domainFile))
                        {
                            foreach (var file in Directory.GetFiles(domainPath))
                            {
                                if (PDDLFileHelper.IsFileProblem(file))
                                {
                                    count++;
                                    run.TotalSizeB += (new FileInfo(domainFile).Length + new FileInfo(file).Length) * _iterations;
                                    run.TotalFiles += 2;
                                    var decl = pddlParser.ParseDecl(new FileInfo(domainFile), new FileInfo(file));
                                    for (int i = 0; i < _iterations; i++)
                                    {
                                        var checkDecl = decl.Copy();
                                        run.Start();
                                        contextualizer.Contexturalise(checkDecl);
                                        run.Stop();
                                    }
                                    if (count > _firstNProblems)
                                        break;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine($"{run.Name}\t Done!");
                return run;
            }));
            Console.WriteLine("PDDL Analysing");
            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("PDDL Analysing", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var pddlParser = new PDDLParser(errorListener);
                var analyser = new PDDLAnalyser(errorListener);
                var domains = Directory.GetDirectories(benchmarks);
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        var domainFile = Path.Combine(domainPath, "domain.pddl");
                        var count = 0;
                        if (File.Exists(domainFile))
                        {
                            foreach (var file in Directory.GetFiles(domainPath))
                            {
                                if (PDDLFileHelper.IsFileProblem(file))
                                {
                                    count++;
                                    run.TotalSizeB += (new FileInfo(domainFile).Length + new FileInfo(file).Length) * _iterations;
                                    run.TotalFiles += 2;
                                    for (int i = 0; i < _iterations; i++)
                                    {
                                        var decl = pddlParser.ParseDecl(new FileInfo(domainFile), new FileInfo(file));
                                        run.Start();
                                        analyser.Analyse(decl);
                                        run.Stop();
                                    }
                                    if (count > _firstNProblems)
                                        break;
                                }
                            }
                        }
                    }
                }
                Console.WriteLine($"{run.Name}\t Done!");
                return run;
            }));
            Console.WriteLine("PDDL Domain Code Generation");
            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("PDDL Domain Code Generation", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var pddlParser = new PDDLParser(errorListener);
                var codeGenerators = new PDDLCodeGenerator(errorListener);
                var domains = Directory.GetDirectories(benchmarks);
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        var domainFile = Path.Combine(domainPath, "domain.pddl");
                        if (File.Exists(domainFile))
                        {
                            var domain = pddlParser.ParseAs<DomainDecl>(File.ReadAllText(domainFile));
                            run.Start();
                            for (int i = 0; i < _iterations; i++)
                            {
                                var file = codeGenerators.Generate(domain);
                                run.TotalSizeB += file.Length;
                                run.TotalFiles++;
                            }
                            run.Stop();
                        }
                    }
                }
                Console.WriteLine($"{run.Name}\t Done!");
                return run;
            }));
            Console.WriteLine("PDDL Problem Code Generation");
            tasks.Add(new Task<FilePerformanceResult>(() =>
            {
                var run = new FilePerformanceResult("PDDL Problem Code Generation", _iterations);
                Console.WriteLine($"{run.Name}\t Started...");
                var errorListener = new ErrorListener(ParseErrorType.Error);
                var pddlParser = new PDDLParser(errorListener);
                var codeGenerators = new PDDLCodeGenerator(errorListener);
                var domains = Directory.GetDirectories(benchmarks);
                int domainCounter = 0;
                foreach (var domainPath in domains)
                {
                    if (TargetDomains.Contains(new DirectoryInfo(domainPath).Name))
                    {
                        Console.WriteLine($"{run.Name}\t {domainCounter++}/{TargetDomains.Count}");
                        int count = 0;
                        foreach (var file in Directory.GetFiles(domainPath))
                        {
                            if (PDDLFileHelper.IsFileProblem(file))
                            {
                                count++;
                                var problem = pddlParser.ParseAs<ProblemDecl>(File.ReadAllText(file));
                                run.Start();
                                for (int i = 0; i < _iterations; i++)
                                {
                                    var fileData = codeGenerators.Generate(problem);
                                    run.TotalSizeB += fileData.Length;
                                    run.TotalFiles++;
                                }
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
            foreach(var task in tasks)
                results.Add(task.Result);
            return results;
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