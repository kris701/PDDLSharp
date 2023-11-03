using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IPlanner : IDisposable
    {
        public SASDecl Declaration { get; }
        public IHeuristic Heuristic { get; }

        public TimeSpan SearchLimit { get; set; }

        public int Generated { get; }
        public double GeneratedPrSecond { get; }
        public int Expanded { get; }
        public double ExpandedPrSecond { get; }
        public int Evaluations { get; }
        public double EvaluationsPrSecond { get; }

        public bool Aborted { get; }
        public TimeSpan SearchTime { get; }

        public ActionPlan Solve();

        public bool Log { get; set; }
        public void LogStarted();
        public void LogTick();
        public void LogAbort();
        public void LogFail();
        public void LogSuccess(ActionPlan plan);
    }
}
