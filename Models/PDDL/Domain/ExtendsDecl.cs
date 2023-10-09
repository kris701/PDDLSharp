using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ExtendsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Extends { get; set; }

        public ExtendsDecl(ASTNode node, INode parent, List<NameExp> extends) : base(node, parent)
        {
            Extends = extends;
        }

        public ExtendsDecl(INode parent, List<NameExp> extends) : base(parent)
        {
            Extends = extends;
        }

        public ExtendsDecl(List<NameExp> extends) : base()
        {
            Extends = extends;
        }

        public ExtendsDecl() : base()
        {
            Extends = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var extend in Extends)
                hash *= extend.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Extends.GetEnumerator();
        }

        public override ExtendsDecl Copy(INode newParent)
        {
            var newNode = new ExtendsDecl(new ASTNode(Start, End, Line, "", ""), newParent, new List<NameExp>());
            foreach (var node in Extends)
                newNode.Extends.Add(node.Copy(newNode));
            return newNode;
        }
    }
}
