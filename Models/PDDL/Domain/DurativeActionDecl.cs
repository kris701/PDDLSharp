using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DurativeActionDecl : BaseNamedWalkableNode, IDecl, IParametized
    {
        public ParameterExp Parameters { get; set; }
        public IExp Condition { get; set; }
        public IExp Effects { get; set; }
        public IExp Duration { get; set; }

        public DurativeActionDecl(ASTNode node, INode? parent, string name, ParameterExp parameters, IExp condition, IExp effects, IExp duration) : base(node, parent, name)
        {
            Parameters = parameters;
            Condition = condition;
            Effects = effects;
            Duration = duration;
        }

        public DurativeActionDecl(INode? parent, string name, ParameterExp parameters, IExp condition, IExp effects, IExp duration) : base(parent, name)
        {
            Parameters = parameters;
            Condition = condition;
            Effects = effects;
            Duration = duration;
        }

        public DurativeActionDecl(string name, ParameterExp parameters, IExp condition, IExp effects, IExp duration) : base(name)
        {
            Parameters = parameters;
            Condition = condition;
            Effects = effects;
            Duration = duration;
        }

        public DurativeActionDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Condition = new AndExp(this, new List<IExp>());
            Effects = new AndExp(this, new List<IExp>());
            Duration = new AndExp(this, new List<IExp>());
        }

        public DurativeActionDecl(INode? parent, string name) : base(parent, name)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Condition = new AndExp(this, new List<IExp>());
            Effects = new AndExp(this, new List<IExp>());
            Duration = new AndExp(this, new List<IExp>());
        }

        public DurativeActionDecl(string name) : base(name)
        {
            Parameters = new ParameterExp(this, new List<NameExp>());
            Condition = new AndExp(this, new List<IExp>());
            Effects = new AndExp(this, new List<IExp>());
            Duration = new AndExp(this, new List<IExp>());
        }

        public override bool Equals(object? obj)
        {
            if (obj is DurativeActionDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!Parameters.Equals(other.Parameters)) return false;
                if (!Condition.Equals(other.Condition)) return false;
                if (!Effects.Equals(other.Effects)) return false;
                if (!Duration.Equals(other.Duration)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash ^= Parameters.GetHashCode();
            hash ^= Condition.GetHashCode();
            hash ^= Effects.GetHashCode();
            hash ^= Duration.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Condition;
            yield return Effects;
            yield return Duration;
        }
        public override DurativeActionDecl Copy(INode? newParent = null)
        {
            var newNode = new DurativeActionDecl(new ASTNode(Line, "", ""), newParent, Name);
            var newParams = Parameters.Copy(newNode);
            var newCondition = ((dynamic)Condition).Copy(newNode);
            var newEffects = ((dynamic)Effects).Copy(newNode);
            var newDuration = ((dynamic)Duration).Copy(newNode);
            newNode.Parameters = newParams;
            newNode.Condition = newCondition;
            newNode.Effects = newEffects;
            newNode.Duration = newDuration;
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            if (Parameters == node && with is ParameterExp param)
                Parameters = param;
            if (Condition == node && with is IExp exp1)
                Condition = exp1;
            if (Effects == node && with is IExp exp2)
                Effects = exp2;
            if (Duration == node && with is IExp exp3)
                Duration = exp3;
        }
    }
}
