using PDDLSharp.Analysers;
using PDDLSharp.CodeGenerators;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Parsers;
using System;
using System.Diagnostics;

namespace PerformanceTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fetching benchmarks!");
            BenchmarkFetcher.CheckAndDownloadBenchmarksAsync();
            Console.WriteLine("Done!");

            RunNTimes(100);
        }

        private static void RunNTimes(int number)
        {
            //var targetDomain = "benchmarks/airport/p50-domain.pddl";
            //var targetProblem = "benchmarks/airport/p50-airport5MUC-p15.pddl";
            var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);
            IAnalyser analyser = new PDDLAnalyser(listener);
            ICodeGenerator generator = new PDDLCodeGenerator(listener);

            generator.Readable = true;

            Stopwatch instanceWatch = new Stopwatch();
            List<long> times = new List<long>() { 0, 0, 0 };
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"Instance {i}");
                instanceWatch.Start();
                var decl = parser.ParseDecl(targetDomain, targetProblem);
                instanceWatch.Stop();
                times[0] += instanceWatch.ElapsedMilliseconds;

                instanceWatch.Restart();
                analyser.Analyse(decl);
                instanceWatch.Stop();
                times[1] += instanceWatch.ElapsedMilliseconds;

                instanceWatch.Restart();
                generator.Generate(decl.Domain, "domain.pddl");
                generator.Generate(decl.Problem, "problem.pddl");
                instanceWatch.Stop();
                times[2] += instanceWatch.ElapsedMilliseconds;
            }
            Console.WriteLine($"Parsing took         {times[0]}ms in total.\t\tAvg {times[0]/number}ms pr instance.");
            Console.WriteLine($"Analysing took       {times[1]}ms in total.\t\tAvg {times[1]/number}ms pr instance.");
            Console.WriteLine($"Code Generation took {times[2]}ms in total.\t\tAvg {times[2]/number}ms pr instance.");
        }
    }
}