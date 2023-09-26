using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models
{
    public class NumericExp : BaseWalkableNode, IExp, INamedNode
    {
        public string Name { get; set; }
        public IExp Arg1 { get; set; }
        public IExp Arg2 { get; set; }

        public NumericExp(ASTNode node, INode parent, string name, IExp arg1, IExp arg2) : base(node, parent)
        {
            Name = name;
            Arg1 = arg1;
            Arg2 = arg2;
        }

        public override string ToString()
        {
            return $"({Name} {Arg1} {Arg2})";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + Name.GetHashCode() + Arg1.GetHashCode() + Arg2.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is NumericExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            var result = new HashSet<INamedNode>();
            if (Name == name)
                result.Add(this);
            result.AddRange(Arg1.FindNames(name));
            result.AddRange(Arg2.FindNames(name));
            return result;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            res.AddRange(Arg1.FindTypes<T>());
            res.AddRange(Arg2.FindTypes<T>());
            return res;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Arg1;
            yield return Arg2;
        }
    }
}
