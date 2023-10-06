using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DerivedDecl : BaseWalkableNode, IDecl
    {
        public PredicateExp Predicate { get; set; }
        public IExp Expression { get; set; }

        public DerivedDecl(ASTNode node, INode parent, PredicateExp predicate, IExp expression) : base(node, parent)
        {
            Predicate = predicate;
            Expression = expression;
        }

        public DerivedDecl(INode parent, PredicateExp predicte, IExp expression) : base(parent)
        {
            Predicate = predicte;
            Expression = expression;
        }

        public DerivedDecl(PredicateExp predicte, IExp expression) : base()
        {
            Predicate = predicte;
            Expression = expression;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Predicate.GetHashCode();
            hash *= Expression.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Predicate;
            yield return Expression;
        }
    }
}
