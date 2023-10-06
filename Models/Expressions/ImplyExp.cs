using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Expressions
{
    public class ImplyExp : BaseWalkableNode, IExp
    {
        public IExp Antecedent { get; set; }
        public IExp Consequent { get; set; }

        public ImplyExp(ASTNode node, INode parent, IExp antecedent, IExp consequent) : base(node, parent)
        {
            Antecedent = antecedent;
            Consequent = consequent;
        }

        public ImplyExp(INode parent, IExp antecedent, IExp Consequent) : base(parent)
        {
            Antecedent = antecedent;
            this.Consequent = Consequent;
        }

        public ImplyExp(IExp antecedent, IExp Consequent) : base()
        {
            Antecedent = antecedent;
            this.Consequent = Consequent;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            hash *= Antecedent.GetHashCode();
            hash *= Consequent.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            yield return Antecedent;
            yield return Consequent;
        }
    }
}
