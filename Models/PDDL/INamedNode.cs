namespace PDDLSharp.Models.PDDL
{
    public interface INamedNode : INode
    {
        public string Name { get; set; }
    }
}
