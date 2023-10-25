using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IPlanner<StateType>
    {
        public DomainDecl Domain { get; }
        public ProblemDecl Problem { get; }

        public int Generated { get; }
        public int Expanded { get; }

        public void PreProcess();
        public ActionPlan Solve(IHeuristic<StateType> h);
    }
}
