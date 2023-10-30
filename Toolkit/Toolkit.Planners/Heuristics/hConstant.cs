using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

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

        public override int GetValue(StateMove parent, IState state, List<ActionDecl> groundedActions)
        {
            Calculated++;
            return Constant;
        }
    }
}
