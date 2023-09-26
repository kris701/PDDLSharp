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
    public class ExtendsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Extends { get; set; }

        public ExtendsDecl(ASTNode node, INode parent, List<NameExp> extends) : base(node, parent)
        {
            Extends = extends;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Extends)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:extends{retStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var extend in Extends)
                res.AddRange(extend.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var ext in Extends)
                res.AddRange(ext.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach(var extend in Extends)
                hash *= extend.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj is ExtendsDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Extends.GetEnumerator();
        }
    }
}
