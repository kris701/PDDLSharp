using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;

namespace PDDLSharp.Toolkit.PlanValidator
{
    public interface IPlanValidator
    {
        public int Step { get; }
        public bool Validate(ActionPlan plan, PDDLDecl decl);
    }
}
