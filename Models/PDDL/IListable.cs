namespace PDDLSharp.Models.PDDL
{
    public interface IListable : IWalkable
    {
        public void Add(INode node);
        public void Remove(INode node);
        public bool Contains(INode node);
        public int Count(INode node);
    }
}
