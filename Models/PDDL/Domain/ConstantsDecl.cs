using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ConstantsDecl : BaseListableNode, IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(ASTNode node, INode? parent, List<NameExp> constants) : base(node, parent)
        {
            Constants = constants;
        }

        public ConstantsDecl(INode? parent, List<NameExp> constants) : base(parent)
        {
            Constants = constants;
        }

        public ConstantsDecl(List<NameExp> constants) : base()
        {
            Constants = constants;
        }

        public ConstantsDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Constants = new List<NameExp>();
        }

        public ConstantsDecl(INode? parent) : base(parent)
        {
            Constants = new List<NameExp>();
        }

        public ConstantsDecl() : base()
        {
            Constants = new List<NameExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is ConstantsDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Constants, other.Constants)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var constant in Constants)
                hash *= constant.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Constants.GetEnumerator();
        }

        public override ConstantsDecl Copy(INode? newParent = null)
        {
            var newNode = new ConstantsDecl(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            foreach (var node in Constants)
                newNode.Constants.Add(node.Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Constants.Count; i++)
            {
                if (Constants[i] == node && with is NameExp name)
                    Constants[i] = name;
            }
        }

        public override void Add(INode node)
        {
            if (node is NameExp exp)
                Constants.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is NameExp exp)
                Constants.Remove(exp);
        }
    }
}
