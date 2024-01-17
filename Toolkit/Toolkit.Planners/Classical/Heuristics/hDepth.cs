using PDDLSharp.Models.SAS;
using PDDLSharp.StateSpaces.SAS;
using PDDLSharp.Toolkit.Planners.Tools;

namespace PDDLSharp.Toolkit.Planners.Classical.Heuristics
{
    /// <summary>
    /// Simply forces the search to be a depth first search
    /// </summary>
    public class hDepth : BaseHeuristic
    {
        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            return parent.hValue - 1;
        }
    }
}
