using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;

namespace PDDLSharp.Toolkit.Planners.Classical.Heuristics
{
    public class hMax : BaseHeuristic
    {
        private FactRPG _graphGenerator;
        public hMax()
        {
            _graphGenerator = new FactRPG();
        }

        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            var max = 0;
            var dict = _graphGenerator.GenerateRelaxedGraph(state, operators);
            foreach (var fact in state.Declaration.Goal)
            {
                if (!dict.ContainsKey(fact))
                    return int.MaxValue;
                var factCost = dict[fact];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                if (factCost > max)
                    max = factCost;
            }
            return max;
        }

        public override void Reset()
        {
            base.Reset();
            _graphGenerator = new FactRPG();
        }
    }
}
