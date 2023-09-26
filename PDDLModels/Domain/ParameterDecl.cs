using PDDLModels.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools;

namespace PDDLModels.Domain
{
    public class ParameterDecl : BaseWalkableNode
    {
        public List<NameExp> Values { get; set; }

        public ParameterDecl(ASTNode node, INode parent, List<NameExp> values) : base(node, parent)
        {
            Values = values;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParameterDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var param in Values)
                res.AddRange(param.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            foreach (var param in Values)
                res.AddRange(param.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var param in Values)
                hash *= param.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}
