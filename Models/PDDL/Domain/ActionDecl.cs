using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ActionDecl : BaseNamedWalkableNode, IDecl, IParametized
    {
        public ParameterExp Parameters { get; set; }
        public IExp Preconditions { get; set; }
        public IExp Effects { get; set; }

        public ActionDecl(ASTNode node, INode parent, string name, ParameterExp parameters, IExp preconditions, IExp effects) : base(node, parent, name)
        {
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }

        public ActionDecl(INode parent, string name, ParameterExp parameters, IExp preconditions, IExp effects) : base(parent, name)
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

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Parameters.GetHashCode();
            hash *= Preconditions.GetHashCode();
            hash *= Effects.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Preconditions;
            yield return Effects;
        }
    }
}
