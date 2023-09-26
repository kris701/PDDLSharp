using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;

namespace PDDL.Models.Expressions
{
    public class AndExp : BaseWalkableNode, IExp
    {
        public List<IExp> Children { get; set; }

        public AndExp(ASTNode node, INode parent, List<IExp> children) : base(node, parent)
        {
            Children = children;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Children)
                retStr += $" {type}{Environment.NewLine}";
            return $"(and{retStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var child in Children)
                res.AddRange(child.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var child in Children)
                res.AddRange(child.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var child in Children)
                hash *= child.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }
    }
}
