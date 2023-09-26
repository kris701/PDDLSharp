using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Domain
{
    public class DurativeActionDecl : BaseWalkableNode, IDecl, INamedNode
    {
        public string Name { get; set; }

        public ParameterDecl Parameters { get; set; }
        public IExp Condition { get; set; }
        public IExp Effects { get; set; }
        public IExp Duration { get; set; }

        public DurativeActionDecl(ASTNode node, INode parent, string name, ParameterDecl parameters, IExp condition, IExp effects, IExp duration) : base(node, parent)
        {
            Name = name;
            Parameters = parameters;
            Condition = condition;
            Effects = effects;
            Duration = duration;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            if (Name == name)
                res.Add(this);
            res.AddRange(Parameters.FindNames(name));
            res.AddRange(Condition.FindNames(name));
            res.AddRange(Effects.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(Parameters.FindTypes<T>());
            res.AddRange(Condition.FindTypes<T>());
            res.AddRange(Effects.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Name.GetHashCode();
            hash *= Parameters.GetHashCode();
            hash *= Condition.GetHashCode();
            hash *= Effects.GetHashCode();
            hash *= Duration.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is DurativeActionDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
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
