using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.SAS.Sections
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
    }
}
