using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;
using PDDL.Models.Expressions;

namespace PDDL.Models.Domain
{
    public class FunctionsDecl : BaseWalkableNode, IDecl
    {
        public List<PredicateExp> Functions { get; set; }

        public FunctionsDecl(ASTNode node, INode parent, List<PredicateExp> functions) : base(node, parent)
        {
            Functions = functions;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Functions)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:functions{retStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var predicate in Functions)
                res.AddRange(predicate.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var pred in Functions)
                res.AddRange(pred.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var pred in Functions)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Functions.GetEnumerator();
        }
    }
}
