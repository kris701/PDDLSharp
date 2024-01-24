using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;

namespace PDDLSharp.Toolkit.PlanValidator
{
    public interface IPlanValidator
    {
        public int Step { get; }
        public string ValidationError { get; }
        public bool Validate(ActionPlan plan, PDDLDecl decl);
    }
}
