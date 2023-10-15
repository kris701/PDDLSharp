using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
{
    public class VersionDecl : BaseSASNode
    {
        public int Version { get; set; }

        public VersionDecl(ASTNode node, int version) : base(node)
        {
            Version = version;
        }
    }
}
