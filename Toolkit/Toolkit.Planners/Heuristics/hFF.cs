using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hFF : BaseHeuristic
    {
        private RelaxedPlanGenerator _graphGenerator;

        public hFF(PDDLDecl declaration)
        {
            _graphGenerator = new RelaxedPlanGenerator(declaration);
        }

        public override int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators)
        {
            Calculated++;
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                state,
                operators);
            if (_graphGenerator.Failed)
                return int.MaxValue;
            return relaxedPlan.Count;
        }
    }
}
