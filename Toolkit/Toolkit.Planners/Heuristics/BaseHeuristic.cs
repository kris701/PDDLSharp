using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    public abstract class BaseHeuristic : IHeuristic
    {
        public int Evaluations { get; internal set; }
        public abstract int GetValue(StateMove parent, ISASState state, List<Operator> operators);
    }
}
