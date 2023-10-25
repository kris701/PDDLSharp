using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hFF : IHeuristic
    {
        public PDDLDecl Declaration { get; }
        private RelaxedPlanningGraphs _graphGenerator;

        public hFF(PDDLDecl declaration)
        {
            Declaration = declaration;
            _graphGenerator = new RelaxedPlanningGraphs();
        }

        public int GetValue(int currentValue, IState state, HashSet<ActionDecl> groundedActions)
        {
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                new RelaxedPDDLStateSpace(Declaration, state.State),
                groundedActions);
            if (_graphGenerator.Failed)
                return int.MaxValue;
            return relaxedPlan.Count;
        }
    }
}
