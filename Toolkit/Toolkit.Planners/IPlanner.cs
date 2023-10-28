using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
