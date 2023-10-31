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
        public int Expanded { get; }
        public int Evaluations { get; }

        public bool Aborted { get; }
        public TimeSpan SearchTime { get; }

        public ActionPlan Solve();
    }
}
