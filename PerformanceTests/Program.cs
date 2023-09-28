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

            IPDDLParser parser = new PDDLParser(null);
            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine($"Parsing... {i}");
                parser.ParseAs<DomainDecl>(targetDomain);
            }
            watch.Stop();
            Console.WriteLine($"Done! Took {watch.ElapsedMilliseconds}ms");
        }
    }
}