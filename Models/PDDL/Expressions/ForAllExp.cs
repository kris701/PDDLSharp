using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class ForAllExp : BaseWalkableNode, IExp
    {
        public ParameterExp Parameters { get; set; }
        public IExp Expression { get; set; }

        public ForAllExp(ASTNode node, INode parent, ParameterExp parameters, IExp expression) : base(node, parent)
        {
            Parameters = parameters;
            Expression = expression;
        }

        public ForAllExp(INode parent, ParameterExp condition, IExp effect) : base(parent)
        {
            Parameters = condition;
            Expression = effect;
        }

        public ForAllExp(ParameterExp condition, IExp effect) : base()
        {
            Parameters = condition;
            Expression = effect;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Parameters.GetHashCode();
            hash *= Expression.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Expression;
        }
    }
}
