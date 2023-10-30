using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">goal count Evaluator</seealso>
    /// </summary>
    public class hGoal : BaseHeuristic
    {
        public hGoal(PDDLDecl declaration)
        {
        }

        public override int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators)
        {
            Calculated++;
            int count = 0;
            foreach (var goal in state.Goals)
                if (state.Contains(goal))
                    count++;
            return state.Goals.Count - count;
        }
    }
}
