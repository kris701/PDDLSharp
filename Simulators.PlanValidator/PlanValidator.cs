using PDDLSharp.Models.Plans;
using PDDLSharp.Models;
using PDDLSharp.States.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Simulators.StateSpace;
using System.Xml.Linq;

namespace PDDLSharp.Simulators.PlanValidator
{
    public class PlanValidator : IPlanValidator
    {
        public PlanValidator()
        {
        }

        public bool Validate(ActionPlan plan, PDDLDecl decl)
        {
            try
            {
                IStateSpaceSimulator simulator = new StateSpaceSimulator(decl);
                foreach (var step in plan.Plan)
                {
                    var argStr = new List<string>();
                    foreach (var arg in step.Arguments)
                        argStr.Add(arg.Name);

                    simulator.Step(step.ActionName, argStr.ToArray());
                }
                return simulator.State.IsInGoal();
            }
            catch
            {
                return false;
            }
        }
    }
}
