using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hFF : BaseHeuristic
    {
        private SASDecl _declaration;
        private RelaxedPlanGenerator _graphGenerator;

        public hFF(SASDecl declaration)
        {
            _declaration = declaration;
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

        public override void Reset()
        {
            base.Reset();
            _graphGenerator = new RelaxedPlanGenerator(_declaration);
        }
    }
}
