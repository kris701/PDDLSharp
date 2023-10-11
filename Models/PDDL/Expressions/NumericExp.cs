using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class NumericExp : BaseNamedWalkableNode, IExp
    {
        public IExp Arg1 { get; set; }
        public IExp Arg2 { get; set; }

        public NumericExp(ASTNode node, INode parent, string name, IExp arg1, IExp arg2) : base(node, parent, name)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public NumericExp(INode parent, string name, IExp arg1, IExp arg2) : base(parent, name)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public NumericExp(string name, IExp arg1, IExp arg2) : base(name)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Arg1.GetHashCode() ^ Arg2.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Arg1;
            yield return Arg2;
        }

        public override NumericExp Copy(INode newParent)
        {
            var newNode = new NumericExp(new ASTNode(Start, End, Line, "", ""), newParent, Name, null, null);
            newNode.Arg1 = ((dynamic)Arg1).Copy(newNode);
            newNode.Arg2 = ((dynamic)Arg2).Copy(newNode);
            return newNode;
        }
    }
}
