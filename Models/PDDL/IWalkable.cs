namespace PDDLSharp.Models.PDDL
{
    public interface IWalkable : INode, IEnumerable<INode>
    {
        public void Replace(INode node, INode with);
    }
}
