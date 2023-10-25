using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.StateSpace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners
{
    public class RelaxedPlanningGraphs
    {
        public bool Failed { get; set; } = false;
        public HashSet<ActionDecl> GenerateReplaxedPlan(IState state, HashSet<ActionDecl> groundedActions)
        {
            Failed = false;
            state = state.Copy();
            HashSet<ActionDecl> executedActions = new HashSet<ActionDecl>();
            while (!state.IsInGoal())
            {
                HashSet<ActionDecl> validActions = new HashSet<ActionDecl>();
                foreach(var act in groundedActions)
                    if (state.IsNodeTrue(act.Preconditions))
                        validActions.Add(act);
                if (validActions.Count == 0)
                {
                    Failed = true;
                    return new HashSet<ActionDecl>();
                }
                int changes = executedActions.Count;
                foreach (var act in validActions)
                {
                    var stepChange = state.ExecuteNode(act.Effects);
                    if (stepChange != 0)
                        executedActions.Add(act);
                }
                if (changes == executedActions.Count)
                {
                    Failed = true;
                    return new HashSet<ActionDecl>();
                }
            }
            return executedActions;
        }
    }
}
