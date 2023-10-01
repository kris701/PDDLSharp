using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Expressions
{
    public class ExistsExp : BaseWalkableNode, IExp
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
