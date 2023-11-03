using PDDLSharp.Models.SAS;
using PDDLSharp.Toolkit.Planners.Search;
using PDDLSharp.Toolkit.StateSpace.SAS;

namespace PDDLSharp.Toolkit.Planners.HeuristicsCollections
{
    public abstract class BaseHeuristicCollection : IHeuristicCollection
    {
        public int Evaluations { get; internal set; }
        public List<IHeuristic> Heuristics { get; set; }

        public BaseHeuristicCollection(List<IHeuristic> heuristics)
        {
            Heuristics = heuristics;
        }

        public BaseHeuristicCollection()
        {
            Heuristics = new List<IHeuristic>();
        }

        public abstract int GetValue(StateMove parent, ISASState state, List<Operator> operators);
        public virtual void Reset()
        {
            Evaluations = 0;
            foreach (var h in Heuristics)
                h.Reset();
        }
    }
}
