using PDDLSharp.Models;
using PDDLSharp.Models.Plans;
using PDDLSharp.Simulators.StateSpace;
using PDDLSharp.States.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.PlanValidator
{
    public interface IPlanValidator
    {
        public int Step { get; }
        public bool Validate(ActionPlan plan, PDDLDecl decl);
    }
}
