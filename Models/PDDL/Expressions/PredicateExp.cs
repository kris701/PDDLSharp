using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class PredicateExp : BaseNamedWalkableNode<PredicateExp>, IExp
    {
        public List<NameExp> Arguments { get; set; }

        public PredicateExp(ASTNode node, INode parent, string name, List<NameExp> arguments) : base(node, parent, name)
        {
            Arguments = arguments;
        }

        public PredicateExp(INode parent, string name, List<NameExp> arguments) : base(parent, name)
        {
            Arguments = arguments;
        }

        public PredicateExp(string name, List<NameExp> arguments) : base(name)
        {
            Arguments = arguments;
        }

        public PredicateExp(string name) : base(name)
        {
            Arguments = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            int hash = base.GetHashCode();
            foreach (var arg in Arguments)
                hash *= arg.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Arguments.GetEnumerator();
        }

        public override PredicateExp Copy(INode newParent)
        {
            var newNode = new PredicateExp(new ASTNode(Start, End, Line, "", ""), newParent, Name, new List<NameExp>());
            foreach (var node in Arguments)
                newNode.Arguments.Add(((dynamic)node).Copy(newNode));
            return newNode;
        }
    }
}
