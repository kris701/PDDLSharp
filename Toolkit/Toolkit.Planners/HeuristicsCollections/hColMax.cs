using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.HeuristicsCollections
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">max Evaluator</seealso>
    /// </summary>
    public class hColMax : BaseHeuristicCollection
    {
        public hColMax(List<IHeuristic> heuristics) : base(heuristics)
        {
        }

        public override int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            int max = -1;
            foreach(var heuristic in Heuristics)
            {
                var hValue = heuristic.GetValue(parent, state, groundedActions);
                if (hValue > max)
                    max = hValue;
            }
            return max;
        }
    }
}
