using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ExistsExp : BaseWalkableNode, IExp, IParametized
    {
        public ParameterExp Parameters { get; set; }
        public IExp Expression { get; set; }

        public ExistsExp(ASTNode node, INode? parent, ParameterExp parameters, IExp expression) : base(node, parent)
        {
            Parameters = parameters;
            Expression = expression;
        }

        public ExistsExp(INode? parent, ParameterExp parameters, IExp expression) : base(parent)
        {
            Parameters = parameters;
            Expression = expression;
        }

        public ExistsExp(ParameterExp parameters, IExp expression) : base()
        {
            Parameters = parameters;
            Expression = expression;
        }

        public ExistsExp(ASTNode node, INode? parent) : base(node, parent)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>());
        }

        public ExistsExp(INode? parent) : base(parent)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>());
        }

        public ExistsExp() : base()
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>());
        }

        public override bool Equals(object? obj)
        {
            if (obj is ExistsExp other)
            {
                if (!base.Equals(other)) return false;
                if (!Parameters.Equals(other.Parameters)) return false;
                if (!Expression.Equals(other.Expression)) return false;
                return true;
            }
            return false;
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

        public override ExistsExp Copy(INode? newParent = null)
        {
            var newNode = new ExistsExp(new ASTNode(Line, "", ""), newParent);
            var newParams = Parameters.Copy(newNode);
            var newExp = ((dynamic)Expression).Copy(newNode);
            newNode.Parameters = newParams;
            newNode.Expression = newExp;
            newNode.IsHidden = IsHidden;
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
