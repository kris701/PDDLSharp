﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DurativeActionDecl : BaseNamedWalkableNode, IDecl
    {
        public ParameterExp Parameters { get; set; }
        public IExp Condition { get; set; }
        public IExp Effects { get; set; }
        public IExp Duration { get; set; }

        public DurativeActionDecl(ASTNode node, INode parent, string name, ParameterExp parameters, IExp condition, IExp effects, IExp duration) : base(node, parent, name)
        {
            Parameters = parameters;
            Condition = condition;
            Effects = effects;
            Duration = duration;
        }

        public DurativeActionDecl(INode parent, string name, ParameterExp parameters, IExp condition, IExp effects, IExp duration) : base(parent, name)
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

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Parameters.GetHashCode();
            hash *= Condition.GetHashCode();
            hash *= Effects.GetHashCode();
            hash *= Duration.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Parameters;
            yield return Condition;
            yield return Effects;
            yield return Duration;
        }
    }
}
