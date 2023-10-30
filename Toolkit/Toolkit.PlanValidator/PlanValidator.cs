using PDDLSharp.Models;
using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Toolkit.StateSpace.PDDL;

namespace PDDLSharp.Toolkit.PlanValidator
{
    public class PlanValidator : IPlanValidator
    {
        public int Step { get; internal set; }

        public PlanValidator()
        {
        }

        public bool Validate(ActionPlan plan, PDDLDecl decl)
        {
            Step = 0;
            IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);
            try
            {
                foreach (var step in plan.Plan)
                {
                    var argStr = new List<string>();
                    foreach (var arg in step.Arguments)
                        argStr.Add(arg.Name);

                    simulator.Step(step.ActionName, argStr.ToArray());
                    Step++;
                }
                return simulator.State.IsInGoal();
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
