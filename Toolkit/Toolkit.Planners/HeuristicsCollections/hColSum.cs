﻿using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.HeuristicsCollections
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">sum Evaluator</seealso>
    /// </summary>
    public class hColSum : BaseHeuristicCollection
    {
        public hColSum() : base()
        {
        }

        public hColSum(List<IHeuristic> heuristics) : base(heuristics)
        {
        }

        public override int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            Calculated++;
            int sum = 0;
            foreach (var heuristic in Heuristics)
                sum += heuristic.GetValue(parent, state, groundedActions);
            return sum;
        }
    }
}
