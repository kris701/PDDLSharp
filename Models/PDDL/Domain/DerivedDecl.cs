using Microsoft.VisualBasic;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DerivedDecl : BaseWalkableNode, IDecl
    {
        public PredicateExp Predicate { get; set; }
        public IExp Expression { get; set; }

        public DerivedDecl(ASTNode node, INode? parent, PredicateExp predicate, IExp expression) : base(node, parent)
        {
            Predicate = predicate;
            Expression = expression;
        }

        public DerivedDecl(INode? parent, PredicateExp predicte, IExp expression) : base(parent)
        {
            Predicate = predicte;
            Expression = expression;
        }

        public DerivedDecl(PredicateExp predicte, IExp expression) : base()
        {
            Predicate = predicte;
            Expression = expression;
        }

        public DerivedDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Predicate = new PredicateExp(this, "", new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>()); ;
        }

        public DerivedDecl(INode? parent) : base(parent)
        {
            Predicate = new PredicateExp(this, "", new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>()); ;
        }

        public DerivedDecl() : base()
        {
            Predicate = new PredicateExp(this, "", new List<NameExp>());
            Expression = new AndExp(this, new List<IExp>()); ;
        }

        public override bool Equals(object? obj)
        {
            if (obj is DerivedDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!Predicate.Equals(other.Predicate)) return false;
                if (!Expression.Equals(other.Expression)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= Predicate.GetHashCode();
            hash ^= Expression.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Predicate;
            yield return Expression;
        }

        public override DerivedDecl Copy(INode? newParent = null)
        {
            var newNode = new DerivedDecl(new ASTNode(Line, "", ""), newParent);
            newNode.Predicate = Predicate.Copy(newParent);
            newNode.Expression = ((dynamic)Expression).Copy(newParent);
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Predicate == node && node is PredicateExp pred)
                Predicate = pred;
            if (Expression == node && node is IExp exp1)
                Expression = exp1;
        }
    }
}
