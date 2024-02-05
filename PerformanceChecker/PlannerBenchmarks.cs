using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Translators;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Toolkit.Planners;
using PDDLSharp.Toolkit.Planners.Search.Classical;
using PDDLSharp.Toolkit.Planners.Heuristics;

namespace PerformanceChecker
{
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class PlannerBenchmarks
    {
        public static string _domain = File.ReadAllText("domain.pddl");
        public static string _problem = File.ReadAllText("prob05.pddl");
        private ITranslator<PDDLDecl, SASDecl> _translator;
        private PDDLDecl _contextPDDLDecl;
        private SASDecl _contextSASDecl;
        private IPlanner _planner1;
        private IPlanner _planner2;

        public PlannerBenchmarks()
        {
            var listener = new ErrorListener();
            var parser = new PDDLParser(listener);
            _contextPDDLDecl = new PDDLDecl(parser.ParseAs<DomainDecl>(_domain), parser.ParseAs<ProblemDecl>(_problem));
            _translator = new PDDLToSASTranslator(true);
            _contextSASDecl = _translator.Translate(_contextPDDLDecl);
            _planner1 = new GreedyBFS(_contextSASDecl, new hGoal());
            _planner2 = new GreedyBFS(_contextSASDecl, new hFF(_contextSASDecl));
        }

        [Benchmark]
        public SASDecl Translation() => _translator.Translate(_contextPDDLDecl);

        [Benchmark]
        public ActionPlan SolveHGoal() => _planner1.Solve();

        [Benchmark]
        public ActionPlan SolveHFF() => _planner2.Solve();
    }
}
