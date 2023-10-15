using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
{
    public class MutexDecl : BaseSASNode
    {
        public List<ValuePair> Group { get; set; }

        public MutexDecl(ASTNode node, List<ValuePair> group) : base(node)
        {
            Group = group;
        }

        public MutexDecl(List<ValuePair> group)
        {
            Group = group;
        }
    }
}
