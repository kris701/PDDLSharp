using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Exceptions;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.Planners.Tools;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.PDDL;
using PDDLSharp.Toolkit.StateSpace.SAS;
using PDDLSharp.Tools;
using System.Threading.Channels;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hAdd : BaseHeuristic
    {
        private FactRPG _graphGenerator;
        public hAdd()
        {
            _graphGenerator = new FactRPG();
        }

        public override int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators)
        {
            Evaluations++;
            var cost = 0;
            var dict = _graphGenerator.GenerateRelaxedGraph(state, operators);
            foreach (var fact in state.Goals)
            {
                var factCost = dict[fact];
                if (factCost == int.MaxValue)
                    return int.MaxValue;
                cost += factCost;
            }
            return cost;
        }
    }
}
