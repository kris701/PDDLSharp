using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PDDLSharp.Analysers.PDDL;
using PDDLSharp.Analysers;
using PDDLSharp.CodeGenerators.PDDL;
using PDDLSharp.CodeGenerators;
using PDDLSharp.Contextualisers.PDDL;
using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Parsers.PDDL;
using PDDLSharp.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Parsers.FastDownward.SAS;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.FastDownward.SAS;

namespace PerformanceChecker
{
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class FDBenchmarks
    {
        public static string _plan = File.ReadAllText("prob05.plan");
        public static string _sas = File.ReadAllText("prob05.sas");
        private IErrorListener _listener1 = new ErrorListener();
        private IParser<ActionPlan> _parser1;
        private IErrorListener _listener2 = new ErrorListener();
        private IParser<ISASNode> _parser2;

        public FDBenchmarks()
        {
            _parser1 = new FDPlanParser(_listener1);
            _parser2 = new FDSASParser(_listener2);
        }

        [Benchmark]
        public ISASNode FDSASParsing() => _parser2.Parse(_sas);

        [Benchmark]
        public ActionPlan SASPlanParsing() => _parser1.Parse(_plan);
    }
}
