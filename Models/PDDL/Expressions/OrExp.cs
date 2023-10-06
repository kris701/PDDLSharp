using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class OrExp : BaseWalkableNode, IExp
    {
        public List<IExp> Options { get; set; }

        public OrExp(ASTNode node, INode parent, List<IExp> options) : base(node, parent)
        {
            Options = options;
        }

        public OrExp(INode parent, List<IExp> options) : base(parent)
        {
            Options = options;
        }

        public OrExp(List<IExp> options) : base()
        {
            Options = options;
        }

        public OrExp() : base()
        {
            Options = new List<IExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var option in Options)
                hash *= option.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Options.GetEnumerator(); ;
        }
    }
}
