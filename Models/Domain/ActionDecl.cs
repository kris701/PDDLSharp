using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Domain
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
