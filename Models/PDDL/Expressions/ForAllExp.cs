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

        public override bool Equals(object? obj)
        {
            if (obj is ForAllExp other)
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

        public override ForAllExp Copy(INode? newParent = null)
        {
            var newNode = new ForAllExp(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
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
