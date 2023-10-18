namespace PDDLSharp.Models.PDDL
{
    public interface IListable : IWalkable
    {
        public void Add(INode node);
        public void AddRange(List<INode> nodes);
        public void Remove(INode node);
        public void RemoveRange(List<INode> nodes);
        public void RemoveAll(INode node);
        public bool Contains(INode node);
        public bool ContainsAll(List<INode> nodes);
        public int Count(INode node);
    }
}
