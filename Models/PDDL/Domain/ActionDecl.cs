using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ActionDecl : BaseNamedWalkableNode, IDecl, IParametized
    {
        public ParameterExp Parameters { get; set; }
        public IExp Preconditions { get; set; }
        public IExp Effects { get; set; }

        public ActionDecl(ASTNode node, INode? parent, string name, ParameterExp parameters, IExp preconditions, IExp effects) : base(node, parent, name)
        {
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }

        public ActionDecl(INode? parent, string name, ParameterExp parameters, IExp preconditions, IExp effects) : base(parent, name)
        {
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }

        public ActionDecl(string name, ParameterExp parameters, IExp preconditions, IExp effects) : base(name)
        {
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }

        public ActionDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Preconditions = new AndExp(this, new List<IExp>());
            Effects = new AndExp(this, new List<IExp>());
        }

        public ActionDecl(INode? parent, string name) : base(parent, name)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Preconditions = new AndExp(this, new List<IExp>());
            Effects = new AndExp(this, new List<IExp>());
        }

        public ActionDecl(string name) : base(name)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Preconditions = new AndExp(this, new List<IExp>());
            Effects = new AndExp(this, new List<IExp>());
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= Parameters.GetHashCode();
            hash ^= Preconditions.GetHashCode();
            hash ^= Effects.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Preconditions;
            yield return Effects;
        }
        public override ActionDecl Copy(INode? newParent = null)
        {
            var newNode = new ActionDecl(new ASTNode(Start, End, Line, "", ""), newParent, Name);
            var newParams = Parameters.Copy(newNode);
            var newPreconditions = ((dynamic)Preconditions).Copy(newNode);
            var newEffects = ((dynamic)Effects).Copy(newNode);
            newNode.Parameters = newParams;
            newNode.Preconditions = newPreconditions;
            newNode.Effects = newEffects;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Parameters == node && with is ParameterExp param)
                Parameters = param;
            if (Preconditions == node && with is IExp exp1)
                Preconditions = exp1;
            if (Effects == node && with is IExp exp2)
                Effects = exp2;
        }

        public override string ToString()
        {
            return Name + Parameters.ToString();
        }
    }
}
