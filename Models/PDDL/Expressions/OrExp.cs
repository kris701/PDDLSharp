using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class OrExp : BaseListableNode, IExp
    {
        public List<IExp> Options { get; set; }

        public OrExp(ASTNode node, INode? parent, List<IExp> options) : base(node, parent)
        {
            Options = options;
        }

        public OrExp(INode? parent, List<IExp> options) : base(parent)
        {
            Options = options;
        }

        public OrExp(List<IExp> options) : base()
        {
            Options = options;
        }

        public OrExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Options = new List<IExp>();
        }

        public OrExp(INode? parent) : base(parent)
        {
            Options = new List<IExp>();
        }

        public OrExp() : base()
        {
            Options = new List<IExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is OrExp other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Options, other.Options)) return false;
                return true;
            }
            return false;
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
            return Options.GetEnumerator();
        }

        public override OrExp Copy(INode? newParent = null)
        {
            var newNode = new OrExp(new ASTNode(Start, End, Line, "", ""), newParent);
            foreach (var node in Options)
                newNode.Options.Add(((dynamic)node).Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i] == node && with is IExp asExp)
                    Options[i] = asExp;
            }
        }

        public override void Add(INode node)
        {
            if (node is IExp exp)
                Options.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is IExp exp)
                Options.Remove(exp);
        }
    }
}
