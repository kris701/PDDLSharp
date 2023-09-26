using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;

namespace PDDL.Models.Expressions
{
    public class OrExp : BaseWalkableNode, IExp
    {
        public IExp Option1 { get; set; }
        public IExp Option2 { get; set; }

        public OrExp(ASTNode node, INode parent, IExp option1, IExp option2) : base(node, parent)
        {
            Option1 = option1;
            Option2 = option2;
        }

        public override string ToString()
        {
            return $"(or {Option1} {Option2})";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * Option1.GetHashCode() * Option2.GetHashCode();
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Option1;
            yield return Option2;
        }
    }
}
