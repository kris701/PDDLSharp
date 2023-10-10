namespace PDDLSharp.Models.PDDL
{
    public interface INode
    {
        public INode? Parent { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }
        public bool IsHidden { get; set; }

        public INode Copy(INode newParent);
        public List<INamedNode> FindNames(string name);
        public void FindNames(List<INamedNode> returnSet, string name);
        public List<T> FindTypes<T>(List<Type>? stopIf = null, bool ignoreFirst = false);
        public void FindTypes<T>(List<T> returnSet, List<Type>? stopIf = null, bool ignoreFirst = false);
        public int GetHashCode();
    }
}
