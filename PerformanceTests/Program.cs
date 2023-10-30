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
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Parsers.SAS;
using PDDLSharp.Toolkit.MacroGenerators;
using PDDLSharp.Toolkit.Planners;
using PDDLSharp.Toolkit.Planners.Heuristics;
using PDDLSharp.Toolkit.Planners.HeuristicsCollections;
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
            GitFetcher.CheckAndDownloadBenchmarksAsync("https://github.com/kris701/PDDLBenchmarkPlans", "benchmarks-plans").Wait();
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
            var targetDomain = "benchmarks/logistics98/domain.pddl";
            var targetProblem = "benchmarks/logistics98/prob01.pddl";
            //var targetDomain = "benchmarks/gripper/domain.pddl";
            //var targetProblem = "benchmarks/gripper/prob01.pddl";

            IErrorListener listener = new ErrorListener();
            PDDLParser parser = new PDDLParser(listener);

            PDDLDecl decl = new PDDLDecl(parser.ParseAs<DomainDecl>(new FileInfo(targetDomain)), parser.ParseAs<ProblemDecl>(new FileInfo(targetProblem)));

            var greedyBFS_UAR = new GreedyBFSUAR(decl);
            var greedyBFS = new GreedyBFS(decl);
            var greedyBFS_PO = new GreedyBFSPO(decl);
            var greedyBFS_DHE = new GreedyBFSDHE(decl);

            var h1 = new hDepth();
            var h2 = new hFF(decl);
            var h3 = new hGoal(decl);
            var h4 = new hConstant(1);
            var h5 = new hPath();
            var h6 = new hAdd(decl);
            var h7 = new hMax(decl);
            var hc5 = new hColMax(new List<IHeuristic>()
            {
                h2,
                h3
            });

            Console.WriteLine($"Grounding...");
            greedyBFS_UAR.PreProcess();
            greedyBFS.GroundedActions = greedyBFS_UAR.GroundedActions;
            greedyBFS_PO.GroundedActions = greedyBFS_UAR.GroundedActions;
            greedyBFS_DHE.GroundedActions = greedyBFS_UAR.GroundedActions;

            Thread.Sleep(1000);

            var actionPlan1 = new ActionPlan(new List<GroundedAction>(), 0);
            var actionPlan2 = new ActionPlan(new List<GroundedAction>(), 0);
            var actionPlan3 = new ActionPlan(new List<GroundedAction>(), 0);
            var actionPlan4 = new ActionPlan(new List<GroundedAction>(), 0);

            Stopwatch instanceWatch = new Stopwatch();
            List<long> times = new List<long>() { 0, 0, 0, 0 };
            for (int i = 0; i < number; i++)
            {
                Console.WriteLine($"Instance {i}");

                Console.WriteLine($"{nameof(greedyBFS_UAR)} using {h2.GetType().Name}");
                instanceWatch.Restart();
                h2 = new hFF(decl);
                h7 = new hMax(decl);
                actionPlan1 = greedyBFS_UAR.Solve(h7);
                instanceWatch.Stop();
                times[0] += instanceWatch.ElapsedMilliseconds;
                Console.WriteLine($"{nameof(greedyBFS_UAR)} calculated heuristic {h2.Calculated} times");

                Console.WriteLine($"{nameof(greedyBFS)} using {h2.GetType().Name}");
                instanceWatch.Restart();
                h2 = new hFF(decl);
                h7 = new hMax(decl);
                actionPlan2 = greedyBFS.Solve(h7);
                instanceWatch.Stop();
                times[1] += instanceWatch.ElapsedMilliseconds;
                Console.WriteLine($"{nameof(greedyBFS)} calculated heuristic {h2.Calculated} times");

                Console.WriteLine($"{nameof(greedyBFS_PO)} using {h2.GetType().Name}");
                instanceWatch.Restart();
                h2 = new hFF(decl);
                h7 = new hMax(decl);
                actionPlan3 = greedyBFS_PO.Solve(h7);
                instanceWatch.Stop();
                times[2] += instanceWatch.ElapsedMilliseconds;
                Console.WriteLine($"{nameof(greedyBFS_PO)} calculated heuristic {h2.Calculated} times");

                Console.WriteLine($"{nameof(greedyBFS_DHE)} using {h2.GetType().Name}");
                instanceWatch.Restart();
                h2 = new hFF(decl);
                h7 = new hMax(decl);
                actionPlan4 = greedyBFS_DHE.Solve(h7);
                instanceWatch.Stop();
                times[3] += instanceWatch.ElapsedMilliseconds;
                Console.WriteLine($"{nameof(greedyBFS_DHE)} calculated heuristic {h2.Calculated} times");
            }

            Console.WriteLine($"{nameof(greedyBFS_UAR)} took {times[0]}ms");
            Console.WriteLine($"{nameof(greedyBFS_UAR)} generated {greedyBFS_UAR.Generated} states and expanded {greedyBFS_UAR.Expanded}");
            Console.WriteLine($"{nameof(greedyBFS_UAR)} had {greedyBFS_UAR.OperatorsUsed} operators to use out of {greedyBFS_UAR.GroundedActions.Count}");
            Console.WriteLine($"{nameof(greedyBFS_UAR)} actually used {actionPlan1.Plan.ToHashSet().Count} operators");
            Console.WriteLine($"{nameof(greedyBFS)} took {times[1]}ms");
            Console.WriteLine($"{nameof(greedyBFS)} generated {greedyBFS.Generated} states and expanded {greedyBFS.Expanded}");
            Console.WriteLine($"{nameof(greedyBFS)} had {greedyBFS.GroundedActions.Count} operators to use out of {greedyBFS.GroundedActions.Count}");
            Console.WriteLine($"{nameof(greedyBFS)} actually used {actionPlan2.Plan.ToHashSet().Count} operators");
            Console.WriteLine($"{nameof(greedyBFS_PO)} took {times[2]}ms");
            Console.WriteLine($"{nameof(greedyBFS_PO)} generated {greedyBFS_PO.Generated} states and expanded {greedyBFS_PO.Expanded}");
            Console.WriteLine($"{nameof(greedyBFS_PO)} had {greedyBFS_PO.GroundedActions.Count} operators to use out of {greedyBFS_PO.GroundedActions.Count}");
            Console.WriteLine($"{nameof(greedyBFS_PO)} actually used {actionPlan3.Plan.ToHashSet().Count} operators");
            Console.WriteLine($"{nameof(greedyBFS_DHE)} took {times[3]}ms");
            Console.WriteLine($"{nameof(greedyBFS_DHE)} generated {greedyBFS_DHE.Generated} states and expanded {greedyBFS_DHE.Expanded}");
            Console.WriteLine($"{nameof(greedyBFS_DHE)} had {greedyBFS_DHE.GroundedActions.Count} operators to use out of {greedyBFS_DHE.GroundedActions.Count}");
            Console.WriteLine($"{nameof(greedyBFS_DHE)} actually used {actionPlan4.Plan.ToHashSet().Count} operators");

            IPlanValidator validator = new PlanValidator();
            Console.WriteLine($"{nameof(greedyBFS_UAR)} plan have {actionPlan1.Cost}");
            if (validator.Validate(actionPlan1, decl))
                Console.WriteLine($"{nameof(greedyBFS_UAR)} plan is valid!");
            else
                Console.WriteLine($"{nameof(greedyBFS_UAR)} plan is NOT valid!");
            Console.WriteLine($"{nameof(greedyBFS)} plan have {actionPlan2.Cost}");
            if (validator.Validate(actionPlan2, decl))
                Console.WriteLine($"{nameof(greedyBFS)} plan is valid!");
            else
                Console.WriteLine($"{nameof(greedyBFS_PO)} plan is NOT valid!");
            Console.WriteLine($"{nameof(greedyBFS_PO)} plan have {actionPlan3.Cost}");
            if (validator.Validate(actionPlan3, decl))
                Console.WriteLine($"{nameof(greedyBFS_PO)} plan is valid!");
            else
                Console.WriteLine($"{nameof(greedyBFS_PO)} plan is NOT valid!");
            Console.WriteLine($"{nameof(greedyBFS_DHE)} plan have {actionPlan4.Cost}");
            if (validator.Validate(actionPlan4, decl))
                Console.WriteLine($"{nameof(greedyBFS_DHE)} plan is valid!");
            else
                Console.WriteLine($"{nameof(greedyBFS_DHE)} plan is NOT valid!");
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