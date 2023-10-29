using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hFF : BaseHeuristic
    {
        private RelaxedPlanGenerator _graphGenerator;

        public hFF(PDDLDecl declaration)
        {
            _graphGenerator = new RelaxedPlanGenerator(declaration);
        }

        public override int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            Calculated++;
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                state,
                groundedActions);
            if (_graphGenerator.Failed)
                return int.MaxValue;
            return relaxedPlan.Count;
        }
    }
}
