using PDDLSharp.Analysers.PDDL;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Tools;
using System;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PerformanceChecker
{
    internal class Program
    {
        public static List<string> TargetDomains = new List<string>()
        {
            "gripper",
            "blocks",
            "depot",
            "logistics98",
            "miconic",
            "rovers",
            "trucks",
            "zenotravel"
        };
        private static int _iterations = 10;
        private static int _firstNProblems = 10;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Fetching Benchmarks");
            var benchmarks = await GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/aibasel/downward-benchmarks/", "benchmarks");
            var benchmarkPlans = await GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/kris701/PDDLBenchmarkPlans", "benchmarks-plans");

            await PDDLPerformance(benchmarks);

        }   
        
        static async Task PDDLPerformance(string benchmarks)
        {
            var tasks = new List<Task<PerformanceResult>>();

            Console.WriteLine("PDDL Domain Parsing");
            tasks.Add(new Task<PerformanceResult>(() =>
            {
                var run = new PerformanceResult("PDDL Domain Parsing", _iterations);
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
                            run.TotalSizeBytes += new FileInfo(domainFile).Length * _iterations;
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
            tasks.Add(new Task<PerformanceResult>(() =>
            {
                var run = new PerformanceResult("PDDL Problem Parsing", _iterations);
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
                                run.TotalSizeBytes += new FileInfo(file).Length * _iterations;
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
            tasks.Add(new Task<PerformanceResult>(() =>
            {
                var run = new PerformanceResult("PDDL Contextualization", _iterations);
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
                                    run.TotalSizeBytes += (new FileInfo(domainFile).Length + new FileInfo(file).Length) * _iterations;
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
            tasks.Add(new Task<PerformanceResult>(() =>
            {
                var run = new PerformanceResult("PDDL Analysing", _iterations);
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
                                    run.TotalSizeBytes += (new FileInfo(domainFile).Length + new FileInfo(file).Length) * _iterations;
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
            tasks.Add(new Task<PerformanceResult>(() =>
            {
                var run = new PerformanceResult("PDDL Domain Code Generation", _iterations);
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
                                run.TotalSizeBytes += file.Length;
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
            tasks.Add(new Task<PerformanceResult>(() =>
            {
                var run = new PerformanceResult("PDDL Problem Code Generation", _iterations);
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
                                    run.TotalSizeBytes += fileData.Length;
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

            foreach(var result in tasks)
            {
                result.Result.Report();
                Console.WriteLine();
            }
        }
    }
}