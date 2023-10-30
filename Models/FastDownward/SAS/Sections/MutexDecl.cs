using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
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
