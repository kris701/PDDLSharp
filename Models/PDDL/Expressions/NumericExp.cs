using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class NumericExp : BaseNamedWalkableNode, IExp
    {
        public IExp Arg1 { get; set; }
        public IExp Arg2 { get; set; }

        public NumericExp(ASTNode node, INode? parent, string name, IExp arg1, IExp arg2) : base(node, parent, name)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public NumericExp(INode? parent, string name, IExp arg1, IExp arg2) : base(parent, name)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public NumericExp(string name, IExp arg1, IExp arg2) : base(name)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public NumericExp(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
            Arg1 = new AndExp(this, new List<IExp>());
            Arg2 = new AndExp(this, new List<IExp>());
        }

        public NumericExp(INode? parent, string name) : base(parent, name)
        {
            Arg1 = new AndExp(this, new List<IExp>());
            Arg2 = new AndExp(this, new List<IExp>());
        }

        public NumericExp(string name) : base(name)
        {
            Arg1 = new AndExp(this, new List<IExp>());
            Arg2 = new AndExp(this, new List<IExp>());
        }

        public override bool Equals(object? obj)
        {
            if (obj is NumericExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Arg1.Equals(other.Arg1)) return false;
                if (!Arg2.Equals(other.Arg2)) return false;
                return true;
            }
            return false;
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

        public override NumericExp Copy(INode? newParent = null)
        {
            var newNode = new NumericExp(new ASTNode(Line, "", ""), newParent, Name);
            newNode.Arg1 = ((dynamic)Arg1).Copy(newNode);
            newNode.Arg2 = ((dynamic)Arg2).Copy(newNode);
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Arg1 == node && with is IExp exp1)
                Arg1 = exp1;
            if (Arg2 == node && with is IExp exp2)
                Arg2 = exp2;
        }
    }
}
