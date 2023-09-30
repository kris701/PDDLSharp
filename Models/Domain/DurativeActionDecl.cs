using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Domain
{
    public class DurativeActionDecl : BaseNamedWalkableNode, IDecl
    {
        public ParameterDecl Parameters { get; set; }
        public IExp Condition { get; set; }
        public IExp Effects { get; set; }
        public IExp Duration { get; set; }

        public DurativeActionDecl(ASTNode node, INode? parent, string name, ParameterDecl parameters, IExp condition, IExp effects, IExp duration) : base(node, parent, name)
        {
            Parameters = parameters;
            Condition = condition;
            Effects = effects;
            Duration = duration;
        }

        public DurativeActionDecl(INode? parent, string name, ParameterDecl parameters, IExp condition, IExp effects, IExp duration) : base(parent, name)
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
