﻿using PDDLSharp.Models.FastDownward.Plans;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Toolkit.Simulators.PDDL;

namespace PDDLSharp.Toolkit.PlanValidators
{
    public class PlanValidator : IPlanValidator
    {
        public int Step { get; internal set; }
        public string ValidationError { get; internal set; } = "";

        public PlanValidator()
        {
        }

        public bool Validate(ActionPlan plan, PDDLDecl decl)
        {
            Step = 0;
            ValidationError = "";
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
                ValidationError = ex.Message;
                return false;
            }
        }
    }
}
