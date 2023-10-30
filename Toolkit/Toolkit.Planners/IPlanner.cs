using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.SAS;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IPlanner : IDisposable
    {
        public PDDLDecl Declaration { get; }
        public List<Operator> Operators { get; set; }
        public IHeuristic Heuristic { get; }

        public TimeSpan PreprocessLimit { get; set; }
        public TimeSpan SearchLimit { get; set; }

        public int Generated { get; }
        public int Expanded { get; }
        public int Evaluations { get; }

        public bool Aborted { get; }
        public TimeSpan PreprocessTime { get; }
        public TimeSpan SearchTime { get; }

        public void PreProcess();
        public ActionPlan Solve();
    }
}
