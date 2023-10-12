using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ForAllExp : BaseWalkableNode, IExp, IParametized
    {
        public ParameterExp Parameters { get; set; }
        public IExp Expression { get; set; }

        public ForAllExp(ASTNode node, INode? parent, ParameterExp parameters, IExp expression) : base(node, parent)
        {
            Parameters = parameters;
            Expression = expression;
        }

        public ForAllExp(INode? parent, ParameterExp condition, IExp effect) : base(parent)
        {
            Parameters = condition;
            Expression = effect;
        }

        public ForAllExp(ParameterExp condition, IExp effect) : base()
        {
            Parameters = condition;
            Expression = effect;
        }

        public ForAllExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>());
        }

        public ForAllExp(INode? parent) : base(parent)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>());
        }

        public ForAllExp() : base()
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>());
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= Parameters.GetHashCode();
            hash ^= Expression.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Expression;
        }

        public override ForAllExp Copy(INode? newParent = null)
        {
            var newNode = new ForAllExp(new ASTNode(Start, End, Line, "", ""), newParent);
            var newParams = Parameters.Copy(newNode);
            var newExp = ((dynamic)Expression).Copy(newNode);
            newNode.Parameters = newParams;
            newNode.Expression = newExp;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Parameters == node && with is ParameterExp param)
                Parameters = param;
            if (Expression == node && with is IExp exp)
                Expression = exp;
        }
    }
}
