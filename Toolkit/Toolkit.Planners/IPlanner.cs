using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL.Domain;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IPlanner
    {
        public PDDLDecl Declaration { get; }
        public List<ActionDecl> GroundedActions { get; set; }

        public TimeSpan Timeout { get; set; }

        public int Generated { get; }
        public int Expanded { get; }

        public void PreProcess();
        public ActionPlan Solve(IHeuristic h);
    }
}
