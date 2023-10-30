﻿using PDDLSharp.Analysers;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.CodeGenerators;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Parsers.FastDownward.SAS;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Toolkit.MacroGenerators;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.PlanValidator;
using System.Diagnostics;

namespace PerformanceTests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fetching benchmarks!");
            GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/aibasel/downward-benchmarks", "benchmarks").Wait();
            //GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/kris701/PDDLBenchmarkPlans", "benchmarks-plans").Wait();
            Console.WriteLine("Done!");

            //RunNTimes(100);
            //RunNTimes2(2000);
            //RunNTimes3(1);
            //RunNTimes4(100);
            //RunNTimes5(50);
            RunNTimes6(1);
        }

        private static void RunNTimes6(int number)
        {
            //var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            //var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";
            //var targetDomain = "benchmarks/barman-sat11-strips/domain.pddl";
            //var targetProblem = "benchmarks/barman-sat11-strips/pfile06-021.pddl";
            //var targetDomain = "benchmarks/tidybot-opt11-strips/domain.pddl";
            //var targetProblem = "benchmarks/tidybot-opt11-strips/p01.pddl";
            //var targetDomain = "benchmarks/logistics98/domain.pddl";
            //var targetProblem = "benchmarks/logistics98/prob35.pddl";
            //var targetDomain = "benchmarks/gripper/domain.pddl";
            //var targetProblem = "benchmarks/gripper/prob20.pddl";
            //var targetDomain = "benchmarks/depot/domain.pddl";
            //var targetProblem = "benchmarks/depot/p11.pddl";
            var targetDomain = "benchmarks/transport-opt08-strips/domain.pddl";
            var targetProblem = "benchmarks/transport-opt08-strips/p05.pddl";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);

            PDDLDecl decl = new PDDLDecl(parser.ParseAs<DomainDecl>(new FileInfo(targetDomain)), parser.ParseAs<ProblemDecl>(new FileInfo(targetProblem)));

            IPlanValidator validator = new PlanValidator();
            using (GreedyBFSUAR planner = new GreedyBFSUAR(decl, new hFF(decl)))
            {
                Stopwatch instanceWatch = new Stopwatch();
                Console.WriteLine($"Grounding...");
                planner.PreProcess();
                Console.WriteLine($"{planner.Operators.Count} total operators");
                Console.WriteLine($"");
                Console.WriteLine($"Solving...");
                instanceWatch.Start();
                var plan = planner.Solve();
                instanceWatch.Stop();
                Console.WriteLine($"");

                Console.WriteLine($"{planner.GetType().Name} took {instanceWatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"{planner.GetType().Name} generated {planner.Generated} states and expanded {planner.Expanded}");
                Console.WriteLine($"{planner.GetType().Name} had {planner.OperatorsUsed} operators to use out of {planner.Operators.Count}");
                Console.WriteLine($"{planner.GetType().Name} heuristic evaluated {planner.Evaluations} times");
                Console.WriteLine($"{planner.GetType().Name} actually used {plan.Plan.Count} operators");

                Console.WriteLine($"");
                Console.WriteLine($"{planner.GetType().Name} plan have a cost of {plan.Cost}");
                if (validator.Validate(plan, decl))
                    Console.WriteLine($"{planner.GetType().Name} plan is valid!");
                else
                    Console.WriteLine($"{planner.GetType().Name} plan is NOT valid!");
            }
            using (GreedyBFS planner = new GreedyBFS(decl, new hFF(decl)))
            {
                Stopwatch instanceWatch = new Stopwatch();
                Console.WriteLine($"Grounding...");
                planner.PreProcess();
                Console.WriteLine($"{planner.Operators.Count} total operators");
                Console.WriteLine($"");
                Console.WriteLine($"Solving...");
                instanceWatch.Start();
                var plan = planner.Solve();
                instanceWatch.Stop();
                Console.WriteLine($"");

                Console.WriteLine($"{planner.GetType().Name} took {instanceWatch.ElapsedMilliseconds}ms");
                Console.WriteLine($"{planner.GetType().Name} generated {planner.Generated} states and expanded {planner.Expanded}");
                Console.WriteLine($"{planner.GetType().Name} heuristic evaluated {planner.Evaluations} times");
                Console.WriteLine($"{planner.GetType().Name} actually used {plan.Plan.Count} operators");

                Console.WriteLine($"");
                Console.WriteLine($"{planner.GetType().Name} plan have a cost of {plan.Cost}");
                if (validator.Validate(plan, decl))
                    Console.WriteLine($"{planner.GetType().Name} plan is valid!");
                else
                    Console.WriteLine($"{planner.GetType().Name} plan is NOT valid!");
            }
        }

        private static void RunNTimes5(int number)
        {
            var targetDomain = "benchmarks/psr-large/domain.pddl";
            var targetProblem = "benchmarks/psr-large/p24-s166-n15-l3-f10.pddl";
            var targetPlans = "benchmarks-plans/lama-first/psr-large/";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);
            FastDownwardPlanParser planParser = new FastDownwardPlanParser(listener);
            var decl = parser.ParseDecl(new FileInfo(targetDomain), new FileInfo(targetProblem));
            IMacroGenerator<List<ActionPlan>> generator = new SequentialMacroGenerator(decl);
            List<ActionPlan> plans = new List<ActionPlan>();
            foreach (var file in new DirectoryInfo(targetPlans).GetFiles())
                if (file.Extension == ".plan")
                    plans.Add(planParser.Parse(file));

            Stopwatch instanceWatch = new Stopwatch();
            instanceWatch.Start();
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"Instance {i}");
                generator.FindMacros(plans);

            }
            instanceWatch.Stop();
            Console.WriteLine($"Done! Took {instanceWatch.ElapsedMilliseconds}ms");
        }

        private static void RunNTimes4(int number)
        {
            var targetDomain = "benchmarks/psr-large/domain.pddl";
            var targetProblem = "benchmarks/psr-large/p24-s166-n15-l3-f10.pddl";
            var targetSAS = "benchmarks-plans/lama-first/psr-large/p24-s166-n15-l3-f10.sas";

            IErrorListener listener = new ErrorListener();
            IParser<ISASNode> sasParser = new SASParser(listener);

            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"Instance {i}");
                var test = sasParser.Parse(new FileInfo(targetSAS));

            }
        }


        private static void RunNTimes3(int number)
        {
            var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";
            var targetPlan = "benchmarks-plans/lama-first/agricola-opt18-strips/p01.plan";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);
            FastDownwardPlanParser planParser = new FastDownwardPlanParser(listener);
            IPlanValidator validator = new PlanValidator();

            Stopwatch instanceWatch = new Stopwatch();
            List<long> times = new List<long>() { 0, 0 };
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"Instance {i}");
                Console.WriteLine($"    Parsing");
                instanceWatch.Start();
                var decl = parser.ParseDecl(new FileInfo(targetDomain), new FileInfo(targetProblem));
                var plan = planParser.Parse(new FileInfo(targetPlan));
                instanceWatch.Stop();
                times[0] += instanceWatch.ElapsedMilliseconds;

                Console.WriteLine($"    Validating");
                instanceWatch.Restart();
                var res = validator.Validate(plan, decl);
                Console.WriteLine($"    Was: {res}");
                instanceWatch.Stop();
                times[1] += instanceWatch.ElapsedMilliseconds;

            }
            Console.WriteLine($"Parsing took         {times[0]}ms in total.\t\tAvg {times[0] / number}ms pr instance.");
            Console.WriteLine($"Validating took      {times[1]}ms in total.\t\tAvg {times[1] / number}ms pr instance.");
        }

        private static void RunNTimes2(int number)
        {
            var targetDomain = "benchmarks/tidybot-opt11-strips/domain.pddl";
            var targetProblem = "benchmarks/tidybot-opt11-strips/p16.pddl";
            //var targetDomain = "benchmarks/agricola-opt18-strips/domain.pddl";
            //var targetProblem = "benchmarks/agricola-opt18-strips/p01.pddl";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);
            var decl = parser.ParseDecl(new FileInfo(targetDomain), new FileInfo(targetProblem));
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
                var decl = parser.ParseDecl(new FileInfo(targetDomain), new FileInfo(targetProblem));
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