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
    public abstract class BaseHeuristicCollection : IHeuristicCollection
    {
        public List<IHeuristic> Heuristics { get; set; }

        public BaseHeuristicCollection(List<IHeuristic> heuristics)
        {
            Heuristics = heuristics;
        }

        public BaseHeuristicCollection()
        {
            Heuristics = new List<IHeuristic>();
        }

        public abstract int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions);
    }
}
