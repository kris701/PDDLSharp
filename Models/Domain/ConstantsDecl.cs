using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;
using PDDL.Models.Expressions;

namespace PDDL.Models.Domain
{
    public class ConstantsDecl : BaseNode, IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(ASTNode node, INode parent, List<NameExp> constants) : base(node, parent) 
        {
            Constants = constants;
        }

        public override string ToString()
        {
            string retStr = "";
            foreach (var type in Constants)
                retStr += $" {type}{Environment.NewLine}";
            return $"(:constants{retStr})";
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();
            foreach (var cons in Constants)
                res.AddRange(cons.FindNames(name));
            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            foreach (var cons in Constants)
                res.AddRange(cons.FindTypes<T>());
            return res;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var constant in Constants)
                hash *= constant.GetHashCode();
            return hash;
        }
    }
}
