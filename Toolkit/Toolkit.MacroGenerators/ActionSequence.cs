using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.MacroGenerators
{
    public class ActionSequence
    {
        public List<GroundedAction> Actions { get; set; }

        public ActionSequence(List<GroundedAction> actions)
        {
            Actions = actions;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ActionSequence op)
                return op.GetHashCode() == GetHashCode();
            return false;
        }

        // The other is important!
        // Based on: https://stackoverflow.com/a/30758270
        public override int GetHashCode()
        {
            const int seed = 487;
            const int modifier = 31;
            unchecked
            {
                return Actions.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.ActionName.GetHashCode());
            }
        }
    }
}
