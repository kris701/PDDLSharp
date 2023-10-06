namespace PDDLSharp.Models.PDDL
{
    public interface INode
    {
        public INode? Parent { get; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }

        public List<INamedNode> FindNames(string name);
        public List<T> FindTypes<T>(List<Type>? stopIf = null);
        public int GetHashCode();
    }
}
