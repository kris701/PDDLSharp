using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ConstantsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(ASTNode node, INode parent, List<NameExp> constants) : base(node, parent)
        {
            Constants = constants;
        }

        public ConstantsDecl(INode parent, List<NameExp> constants) : base(parent)
        {
            Constants = constants;
        }

        public ConstantsDecl(List<NameExp> constants) : base()
        {
            Constants = constants;
        }

        public ConstantsDecl() : base()
        {
            Constants = new List<NameExp>();
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

        public override ConstantsDecl Copy(INode newParent)
        {
            var newNode = new ConstantsDecl(new ASTNode(Start, End, Line, "", ""), newParent, new List<NameExp>());
            foreach (var node in Constants)
                newNode.Constants.Add(node.Copy(newNode));
            return newNode;
        }
    }
}
