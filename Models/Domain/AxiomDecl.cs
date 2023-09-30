using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.Domain
{
    public class AxiomDecl : BaseWalkableNode, IDecl
    {
        public ParameterDecl Vars { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(ASTNode node, INode? parent, ParameterDecl vars, IExp context, IExp implies) : base(node, parent)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }

        public AxiomDecl(INode? parent, ParameterDecl vars, IExp context, IExp implies) : base(parent)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Vars.GetHashCode();
            hash *= Context.GetHashCode();
            hash *= Implies.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Vars;
            yield return Context;
            yield return Implies;
        }
    }
}
