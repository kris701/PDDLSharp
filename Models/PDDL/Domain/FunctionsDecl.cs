using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class FunctionsDecl : BaseListableNode, IDecl
    {
        public List<PredicateExp> Functions { get; set; }

        public FunctionsDecl(ASTNode node, INode? parent, List<PredicateExp> functions) : base(node, parent)
        {
            Functions = functions;
        }

        public FunctionsDecl(INode? parent, List<PredicateExp> functions) : base(parent)
        {
            Functions = functions;
        }

        public FunctionsDecl(List<PredicateExp> functions) : base()
        {
            Functions = functions;
        }

        public FunctionsDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Functions = new List<PredicateExp>();
        }

        public FunctionsDecl(INode? parent) : base(parent)
        {
            Functions = new List<PredicateExp>();
        }

        public FunctionsDecl() : base()
        {
            Functions = new List<PredicateExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is FunctionsDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Functions, other.Functions)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var pred in Functions)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Functions.GetEnumerator();
        }

        public override FunctionsDecl Copy(INode? newParent = null)
        {
            var newNode = new FunctionsDecl(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            foreach (var node in Functions)
                newNode.Functions.Add(node.Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Functions.Count; i++)
            {
                if (Functions[i] == node && with is PredicateExp pred)
                    Functions[i] = pred;
            }
        }

        public override void Add(INode node)
        {
            if (node is PredicateExp exp)
                Functions.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is PredicateExp exp)
                Functions.Remove(exp);
        }
    }
}
