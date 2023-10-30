using PDDLSharp.Models.AST;
using PDDLSharp.Models.FastDownward.SAS;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class OperatorDecl : BaseSASNode
    {
        public string Name { get; set; }
        public List<ValuePair> PrevailConditions { get; set; }
        public List<OperatorEffect> Effects { get; set; }
        public int Cost { get; set; }

        public OperatorDecl(ASTNode node, string name, List<ValuePair> prevailConditions, List<OperatorEffect> effects, int cost) : base(node)
        {
            Name = name;
            PrevailConditions = prevailConditions;
            Effects = effects;
            Cost = cost;
        }

        public OperatorDecl(string name, List<ValuePair> prevailConditions, List<OperatorEffect> effects, int cost)
        {
            Name = name;
            PrevailConditions = prevailConditions;
            Effects = effects;
            Cost = cost;
        }

        public override string? ToString()
        {
            return $"{Name}";
        }
    }
}
