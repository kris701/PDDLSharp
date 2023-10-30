using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IPlanner
    {
        public PDDLDecl Declaration { get; }
        public List<Operator> Operators { get; set; }

        public TimeSpan Timeout { get; set; }

        public int Generated { get; }
        public int Expanded { get; }

        public void PreProcess();
        public ActionPlan Solve(IHeuristic h);
    }
}
