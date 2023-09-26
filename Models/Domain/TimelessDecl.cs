using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Domain
{
    public class TimelessDecl : BaseWalkableNode, IDecl
    {
        public List<PredicateExp> Items { get; set; }

        public TimelessDecl(ASTNode node, INode parent, List<PredicateExp> timeless) : base(node, parent)
        {
            Items = timeless;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Items)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:timeless{retStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var item in Items)
                res.AddRange(item.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var item in Items)
                res.AddRange(item.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var item in Items)
                hash *= item.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is TimelessDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
