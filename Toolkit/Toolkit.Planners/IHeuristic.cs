using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IHeuristic
    {
        public int Calculated { get; }
        public int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators);
    }
}
