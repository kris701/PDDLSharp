using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers
{
    public class FastDownwardPlanParser : BaseParser<ActionPlan>
    {
        public FastDownwardPlanParser(IErrorListener listener) : base(listener)
        {
        }

        public override ActionPlan Parse(string file)
        {
            return new ActionPlan(new List<GroundedAction>(), 0);
        }
    }
}
