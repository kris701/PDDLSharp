using PDDLSharp.Analysers;
using PDDLSharp.CodeGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Parsers;
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
            //RunNTimes2(2000);
        }

        private static void RunNTimes2(int number)
        {
            var targetDomain = "benchmarks/tidybot-opt11-strips/domain.pddl";
            var targetProblem = "benchmarks/tidybot-opt11-strips/p16.pddl";
            //var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            //var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(targetDomain, targetProblem);
            Stopwatch instanceWatch = new Stopwatch();
            instanceWatch.Start();
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"Instance {i}");
                decl.Domain.FindTypes<NameExp>();
            }
            instanceWatch.Stop();
            Console.WriteLine($"Took {instanceWatch.ElapsedMilliseconds} ms");
        }

        private static void RunNTimes(int number)
        {
            var targetDomain = "benchmarks/tidybot-opt11-strips/domain.pddl";
            var targetProblem = "benchmarks/tidybot-opt11-strips/p16.pddl";
            //var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            //var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);
            IAnalyser analyser = new PDDLAnalyser(listener);
            ICodeGenerator<INode> generator = new PDDLCodeGenerator(listener);

            generator.Readable = true;

            Stopwatch instanceWatch = new Stopwatch();
            List<long> times = new List<long>() { 0, 0, 0, 0 };
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"Instance {i}");
                instanceWatch.Start();
                var decl = parser.ParseDecl(targetDomain, targetProblem);
                instanceWatch.Stop();
                times[0] += instanceWatch.ElapsedMilliseconds;

                instanceWatch.Restart();
                var copyDecl = decl.Copy();
                instanceWatch.Stop();
                times[1] += instanceWatch.ElapsedMilliseconds;

                instanceWatch.Restart();
                analyser.Analyse(decl);
                instanceWatch.Stop();
                times[2] += instanceWatch.ElapsedMilliseconds;

                instanceWatch.Restart();
                generator.Generate(decl.Domain, "domain.pddl");
                generator.Generate(decl.Problem, "problem.pddl");
                instanceWatch.Stop();
                times[3] += instanceWatch.ElapsedMilliseconds;
            }
            Console.WriteLine($"Parsing took         {times[0]}ms in total.\t\tAvg {times[0] / number}ms pr instance.");
            Console.WriteLine($"Copying took         {times[1]}ms in total.\t\tAvg {times[1] / number}ms pr instance.");
            Console.WriteLine($"Analysing took       {times[2]}ms in total.\t\tAvg {times[2] / number}ms pr instance.");
            Console.WriteLine($"Code Generation took {times[3]}ms in total.\t\tAvg {times[3] / number}ms pr instance.");
        }
    }
}