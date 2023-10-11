using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ParameterExp : BaseWalkableNode
    {
        public List<NameExp> Values { get; set; }

        public ParameterExp(ASTNode node, INode? parent, List<NameExp> values) : base(node, parent)
        {
            Values = values;
        }

        public ParameterExp(INode? parent, List<NameExp> values) : base(parent)
        {
            Values = values;
        }

        public ParameterExp(List<NameExp> values) : base()
        {
            Values = values;
        }

        public ParameterExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Values = new List<NameExp>();
        }

        public ParameterExp(INode? parent) : base(parent)
        {
            Values = new List<NameExp>();
        }

        public ParameterExp() : base()
        {
            Values = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var param in Values)
                hash *= param.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        public override ParameterExp Copy(INode? newParent = null)
        {
            var newNode = new ParameterExp(new ASTNode(Start, End, Line, "", ""), newParent);
            foreach (var node in Values)
                newNode.Values.Add(((dynamic)node).Copy(newNode));
            return newNode;
        }
    }
}
