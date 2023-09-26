using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;

namespace PDDL.Models.Domain
{
    public class AxiomDecl : BaseWalkableNode, IDecl
    {
        public ParameterDecl Vars { get; set; }
        public IExp Context { get; set; }
        public IExp Implies { get; set; }

        public AxiomDecl(ASTNode node, INode parent, ParameterDecl vars, IExp context, IExp implies) : base(node, parent)
        {
            Vars = vars;
            Context = context;
            Implies = implies;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            res.AddRange(Vars.FindNames(name));
            res.AddRange(Context.FindNames(name));
            res.AddRange(Implies.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(Vars.FindTypes<T>());
            res.AddRange(Context.FindTypes<T>());
            res.AddRange(Implies.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Vars.GetHashCode();
            hash *= Context.GetHashCode();
            hash *= Implies.GetHashCode();
            return hash;
        }

        public override bool Equals(object? obj)
        {
            if (obj is AxiomDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Vars;
            yield return Context;
            yield return Implies;
        }
    }
}
