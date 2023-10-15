using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
{
    public class InitStateDecl : BaseSASNode
    {
        public List<int> Inits { get; set; }

        public InitStateDecl(ASTNode node, List<int> inits) : base(node)
        {
            Inits = inits;
        }

        public override string? ToString()
        {
            var retStr = "";
            foreach(var value in Inits)
                retStr += $"{value} ";
            return retStr.Trim();
        }
    }
}
