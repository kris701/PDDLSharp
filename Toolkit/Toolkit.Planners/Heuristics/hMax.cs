using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hMax : hAdd
    {
        public hMax() : base()
        {
        }

        public override int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators)
        {
            Calculated++;
            var max = 0;
            var dict = GenerateCostStructure(state, operators);
            foreach (var fact in state.Goals)
            {
                var factCost = dict[fact];
                if (factCost == int.MaxValue - 1)
                    return int.MaxValue;
                if (factCost > max)
                    max = factCost;
            }
            return max;
        }
    }
}
