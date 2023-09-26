using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Problem
{
    public class InitDecl : BaseWalkableNode, IDecl
    {
        public List<IExp> Predicates { get; set; }

        public InitDecl(ASTNode node, INode parent, List<IExp> predicates) : base(node, parent)
        {
            Predicates = predicates;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Predicates)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:init{retStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var predicate in Predicates)
                res.AddRange(predicate.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var pred in Predicates)
                res.AddRange(pred.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach(var pred in Predicates)
                hash *= pred.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is InitDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Predicates.GetEnumerator();
        }
    }
}
