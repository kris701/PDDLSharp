using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class TypesDecl : BaseWalkableNode, IDecl
    {
        public List<TypeExp> Types { get; set; }

        public TypesDecl(ASTNode node, INode parent, List<TypeExp> types) : base(node, parent)
        {
            Types = types;
        }

        public TypesDecl(INode parent, List<TypeExp> types) : base(parent)
        {
            Types = types;
        }

        public TypesDecl(List<TypeExp> types) : base()
        {
            Types = types;
        }

        public TypesDecl() : base()
        {
            Types = new List<TypeExp>();
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

        public override TypesDecl Copy(INode newParent)
        {
            var newNode = new TypesDecl(new ASTNode(Start, End, Line, "", ""), newParent, new List<TypeExp>());
            foreach (var node in Types)
                newNode.Types.Add(node.Copy(newNode));
            return newNode;
        }
    }
}
