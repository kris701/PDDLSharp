using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class PredicateExp : BaseNamedWalkableNode, IExp
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

        // The other is important!
        // Based on: https://stackoverflow.com/a/30758270
        public override int GetHashCode()
        {
            const int seed = 487;
            const int modifier = 31;
            unchecked
            {
                return base.GetHashCode() * Arguments.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
            }
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
