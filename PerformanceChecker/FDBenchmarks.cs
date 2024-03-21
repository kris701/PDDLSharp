using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Parsers;
using PDDLSharp.Parsers.FastDownward.Plans;
using PDDLSharp.Parsers.FastDownward.SAS;

namespace PerformanceChecker
{
    [SimpleJob(RuntimeMoniker.HostProcess)]
    public class FDBenchmarks
    {
        public static string _plan = File.ReadAllText("prob05.plan");
        public static string _sas = File.ReadAllText("prob05.sas");
        private readonly IErrorListener _listener1 = new ErrorListener();
        private readonly IParser<ActionPlan> _parser1;
        private readonly IErrorListener _listener2 = new ErrorListener();
        private readonly IParser<ISASNode> _parser2;

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
