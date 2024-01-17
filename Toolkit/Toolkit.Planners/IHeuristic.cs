using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;

namespace PDDLSharp.Toolkit.Planners
{
    public interface IHeuristic
    {
        public int Evaluations { get; }
        public int GetValue(StateMove parent, ISASState state, List<Operator> operators);
        public void Reset();
    }
}
