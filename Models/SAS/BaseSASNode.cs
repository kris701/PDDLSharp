using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS
{
    public class BaseSASNode : INode
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }

        public BaseSASNode(ASTNode node)
        {
            Line = node.Line;
            Start = node.Start;
            End = node.End;
        }

        public BaseSASNode()
        {
            Line = -1;
            Start = -1;
            End = -1;
        }
    }
}
