using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.FastDownward.SAS
{
    public abstract class BaseSASNode : ISASNode
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
