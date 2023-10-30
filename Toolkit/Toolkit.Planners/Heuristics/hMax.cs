using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public class hMax : hAdd
    {
        public hMax(PDDLDecl declaration) : base(declaration)
        {
        }

        public override int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            Calculated++;
            var max = 0;
            var dict = GenerateCostStructure(state, groundedActions);
            foreach (var fact in _goalCache)
            {
                var factCost = dict[fact];
                if (factCost > max)
                    max = factCost;
            }
            return max;
        }
    }
}
