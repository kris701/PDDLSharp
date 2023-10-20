using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Grounders
{
    public class ActionGrounder : BaseGrounder<ActionDecl, GroundedAction>
    {
        public ActionGrounder(PDDLDecl declaration) : base(declaration)
        {
        }

        public override List<GroundedAction> Ground(ActionDecl item)
        {
            List<GroundedAction> groundedActions = new List<GroundedAction>();

            if (item.Parameters.Values.Count == 0)
                return new List<GroundedAction>() { new GroundedAction(item) };

            var allPermuations = GenerateParameterPermutations(item.Parameters.Values);
            foreach (var premutation in allPermuations)
                groundedActions.Add(new GroundedAction(item.Name, premutation.ToArray()));

            return groundedActions;
        }
    }
}
