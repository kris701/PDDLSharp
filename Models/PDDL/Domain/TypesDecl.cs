using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class TypesDecl : BaseListableNode, IDecl
    {
        public List<TypeExp> Types { get; set; }

        public TypesDecl(ASTNode node, INode? parent, List<TypeExp> types) : base(node, parent)
        {
            Types = types;
        }

        public TypesDecl(INode? parent, List<TypeExp> types) : base(parent)
        {
            Types = types;
        }

        public TypesDecl(List<TypeExp> types) : base()
        {
            Types = types;
        }

        public TypesDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Types = new List<TypeExp>();
        }

        public TypesDecl(INode? parent) : base(parent)
        {
            Types = new List<TypeExp>();
        }

        public TypesDecl() : base()
        {
            Types = new List<TypeExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is TypesDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Types, other.Types)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var type in Types)
                hash *= type.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Types.GetEnumerator();
        }

        public override TypesDecl Copy(INode? newParent = null)
        {
            var newNode = new TypesDecl(new ASTNode(Start, End, Line, "", ""), newParent);
            foreach (var node in Types)
                newNode.Types.Add(node.Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Types.Count; i++)
            {
                if (Types[i] == node && with is TypeExp type)
                    Types[i] = type;
            }
        }

        public override void Add(INode node)
        {
            if (node is TypeExp exp)
                Types.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is TypeExp exp)
                Types.Remove(exp);
        }
    }
}
