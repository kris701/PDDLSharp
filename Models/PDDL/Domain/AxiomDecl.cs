using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class AxiomDecl : BaseWalkableNode, IDecl, IParametized
    {
        public ParameterExp Parameters { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(ASTNode node, INode? parent, ParameterExp vars, IExp context, IExp implies) : base(node, parent)
        {
            Parameters = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(INode? parent, ParameterExp vars, IExp context, IExp implies) : base(parent)
        {
            Parameters = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(ParameterExp vars, IExp context, IExp implies) : base()
        {
            Parameters = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Context = new AndExp(this, new List<IExp>());
            Implies = new AndExp(this, new List<IExp>());
        }

        public AxiomDecl(INode? parent) : base(parent)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Context = new AndExp(this, new List<IExp>());
            Implies = new AndExp(this, new List<IExp>());
        }

        public AxiomDecl() : base()
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Context = new AndExp(this, new List<IExp>());
            Implies = new AndExp(this, new List<IExp>());
        }

        public override bool Equals(object? obj)
        {
            if (obj is AxiomDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!Parameters.Equals(other.Parameters)) return false;
                if (!Context.Equals(other.Context)) return false;
                if (!Implies.Equals(other.Implies)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= Parameters.GetHashCode();
            hash ^= Context.GetHashCode();
            hash ^= Implies.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Context;
            yield return Implies;
        }

        public override AxiomDecl Copy(INode? newParent = null)
        {
            var newNode = new AxiomDecl(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            var newParams = Parameters.Copy(newNode);
            var newContext = ((dynamic)Context).Copy(newNode);
            var newImplies = ((dynamic)Implies).Copy(newNode);
            newNode.Parameters = newParams;
            newNode.Context = newContext;
            newNode.Implies = newImplies;
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Parameters == node && with is ParameterExp param)
                Parameters = param;
            if (Context == node && with is IExp exp1)
                Context = exp1;
            if (Implies == node && with is IExp exp2)
                Implies = exp2;
        }
    }
}
