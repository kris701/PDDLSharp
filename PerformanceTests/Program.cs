using PDDLSharp.Analysers;
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

            var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";

            IErrorListener listener = new ErrorListener();
            IPDDLParser parser = new PDDLParser(listener);
            IContextualiser<PDDLDecl> contextualiser = new PDDLDeclContextualiser(listener);
            IAnalyser<PDDLDecl> analyser = new PDDLDeclAnalyser(listener);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"Parsing... {i}");
                var decl = parser.Parse(targetDomain, targetProblem);
                contextualiser.Contexturalise(decl);
                analyser.PostAnalyse(decl);
            }
            watch.Stop();
            Console.WriteLine($"Done! Took {watch.ElapsedMilliseconds}ms");
        }
    }
}