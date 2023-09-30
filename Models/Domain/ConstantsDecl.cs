using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Models.Expressions;

namespace PDDLSharp.Models.Domain
{
    public class ConstantsDecl : BaseNode, IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(ASTNode node, INode? parent, List<NameExp> constants) : base(node, parent) 
        {
            Constants = constants;
        }

        public ConstantsDecl(INode? parent, List<NameExp> constants) : base(parent)
        {
            Constants = constants;
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
