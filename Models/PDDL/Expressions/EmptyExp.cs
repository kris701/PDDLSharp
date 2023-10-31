using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class EmptyExp : BaseNode
    {
        public override INode Copy(INode? newParent = null)
        {
            return new EmptyExp();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
