using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hMax : BaseHeuristic
    {
        private FactRPG _graphGenerator;
        public hMax()
        {
            _graphGenerator = new FactRPG();
        }

        public override int GetValue(StateMove parent, IState<Fact, Operator, SASDecl> state, List<Operator> operators)
        {
            Evaluations++;
            var max = 0;
            var dict = _graphGenerator.GenerateRelaxedGraph(state, operators);
            foreach (var fact in state.Declaration.Goal)
            {
                var factCost = dict[fact];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                if (factCost > max)
                    max = factCost;
            }
            return max;
        }
    }
}
