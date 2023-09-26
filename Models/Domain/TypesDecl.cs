using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Domain
{
    public class TypesDecl : BaseWalkableNode, IDecl
    {
        public List<TypeExp> Types { get; set; }

        public TypesDecl(ASTNode node, INode parent, List<TypeExp> types) : base(node, parent)
        {
            Types = types;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach(var type in Types)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:types{retStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var type in Types)
                res.AddRange(type.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var typeD in Types)
                res.AddRange(typeD.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var type in Types)
                hash *= type.GetHashCode();
            return hash;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TypesDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Types.GetEnumerator();
        }
    }
}
