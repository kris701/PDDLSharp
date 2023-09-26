using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Domain
{
    public class ActionDecl : BaseWalkableNode, IDecl, INamedNode
    {
        public string Name { get; set; }

        public ParameterDecl Parameters { get; set; }
        public IExp Preconditions { get; set; }
        public IExp Effects { get; set; }

        public ActionDecl(ASTNode node, INode parent, string name, ParameterDecl parameters, IExp preconditions, IExp effects) : base(node, parent)
        {
            Name = name;
            Parameters = parameters;
            Preconditions = preconditions;
            Effects = effects;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            if (Name == name)
                res.Add(this);
            foreach (var param in Parameters)
                res.AddRange(param.FindNames(name));
            res.AddRange(Preconditions.FindNames(name));
            res.AddRange(Effects.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(Parameters.FindTypes<T>());
            res.AddRange(Preconditions.FindTypes<T>());
            res.AddRange(Effects.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Name.GetHashCode();
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
