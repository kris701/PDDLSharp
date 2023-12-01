using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.FastDownward.SAS
{
    public abstract class BaseSASNode : ISASNode
    {
        public int Line { get; set; }

        public BaseSASNode(ASTNode node)
        {
            Line = node.Line;
        }

        public BaseSASNode()
        {
            Line = -1;
        }
    }
}
