using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public abstract class BaseHeuristic : IHeuristic
    {
        public int Calculated { get; internal set; }
        public abstract int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions);
    }
}
