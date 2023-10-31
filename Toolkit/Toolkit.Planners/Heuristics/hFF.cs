using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hFF : BaseHeuristic
    {
        private RelaxedPlanGenerator _graphGenerator;

        public hFF(SASDecl declaration)
        {
            _graphGenerator = new RelaxedPlanGenerator(declaration);
        }

        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            var relaxedPlan = _graphGenerator.GenerateReplaxedPlan(
                state,
                operators);
            if (_graphGenerator.Failed)
                return int.MaxValue;
            return relaxedPlan.Count;
        }
    }
}
