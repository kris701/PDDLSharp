namespace PDDLSharp.Toolkit.Planners
{
    public interface IHeuristicCollection : IHeuristic
    {
        public List<IHeuristic> Heuristics { get; set; }
    }
}
