using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public abstract class BaseHeuristic : IHeuristic
    {
        public int Evaluations { get; internal set; }
        public abstract int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators);
    }
}
