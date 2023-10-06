using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ConstantsDecl : BaseNode, IDecl
    {
        public List<NameExp> Constants { get; set; }

        public ConstantsDecl(ASTNode node, INode parent, List<NameExp> constants) : base(node, parent)
        {
            Constants = constants;
        }

        public ConstantsDecl(INode parent, List<NameExp> constants) : base(parent)
        {
            Constants = constants;
        }

        public ConstantsDecl(List<NameExp> constants) : base()
        {
            Constants = constants;
        }

        public ConstantsDecl() : base()
        {
            Constants = new List<NameExp>();
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
