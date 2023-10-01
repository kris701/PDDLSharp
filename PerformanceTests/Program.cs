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

            //var targetDomain = "benchmarks/airport/p50-domain.pddl";
            //var targetProblem = "benchmarks/airport/p50-airport5MUC-p15.pddl";
            var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";

            IErrorListener listener = new ErrorListener();
            IParser parser = new PDDLParser(listener);
            IContextualiser<PDDLDecl> contextualiser = new PDDLDeclContextualiser(listener);
            IAnalyser analyser = new PDDLAnalyser(listener);
            ICodeGenerator generator = new PDDLCodeGenerator(listener);

            generator.Readable = true;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 200; i++)
            {
                Console.WriteLine($"Parsing... {i}");
                var decl = parser.Parse(targetDomain, targetProblem);
                contextualiser.Contexturalise(decl);
                analyser.Analyse(decl);

                generator.Generate(decl.Domain, "domain.pddl");
                generator.Generate(decl.Problem, "problem.pddl");
            }
            watch.Stop();
            Console.WriteLine($"Done! Took {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Avg took {watch.ElapsedMilliseconds/200}ms");
        }
    }
}