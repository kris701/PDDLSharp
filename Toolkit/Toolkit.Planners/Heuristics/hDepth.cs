using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    /// <summary>
    /// Simply forces the search to be a depth first search
    /// </summary>
    public class hDepth : BaseHeuristic
    {
        public override int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            Calculated++;
            return parent.hValue - 1;
        }
    }
}
