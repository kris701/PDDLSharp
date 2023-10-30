using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners.Heuristics
{
    /// <summary>
    /// Based on the <seealso href="https://www.fast-downward.org/Doc/Evaluator">weighted Evaluator</seealso>
    /// </summary>
    public class hWeighted : BaseHeuristic
    {
        public IHeuristic Heuristic { get; set; }
        public double Weight { get; set; }

        public hWeighted(IHeuristic heuristic, double weight)
        {
            Heuristic = heuristic;
            Weight = weight;
        }

        public override int GetValue(StateMove parent, IState<Fact, Operator> state, List<Operator> operators)
        {
            Evaluations++;
            return (int)((double)Heuristic.GetValue(parent, state, operators) * Weight);
        }
    }
}
