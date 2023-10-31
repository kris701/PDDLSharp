using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">constant Evaluator</seealso>
    /// </summary>
    public class hConstant : BaseHeuristic
    {
        public int Constant { get; set; }

        public hConstant(int constant)
        {
            Constant = constant;
        }

        public hConstant()
        {
            Constant = 1;
        }

        public override int GetValue(StateMove parent, ISASState state, List<Operator> operators)
        {
            Evaluations++;
            return Constant;
        }
    }
}
