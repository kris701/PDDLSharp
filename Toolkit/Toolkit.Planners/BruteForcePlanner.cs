using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.Plans;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;

namespace PDDLSharp.Toolkit.Planners
{
    public class BruteForcePlanner : IPlanner
    {
        public DomainDecl Domain { get; }
        public ProblemDecl Problem { get; }

        public BruteForcePlanner(DomainDecl domain, ProblemDecl problem)
        {
            Domain = domain;
            Problem = problem;
        }

        public ActionPlan Solve(IHeuristic h)
        {
            IGrounder<IParametized> grounder = new ParametizedGrounder(new PDDLDecl(Domain, Problem));
            List<IParametized> groundedActions = new List<IParametized>();
            foreach (var action in Domain.Actions)
                groundedActions.AddRange(grounder.Ground(action));

            PDDLStateSpace state = new PDDLStateSpace(new PDDLDecl(Domain, Problem));
            List<GroundedAction> actionSteps = new List<GroundedAction>();
            while (!state.IsInGoal())
            {
                int[] evaluated = new int[groundedActions.Count];
                for(int i = 0; i < groundedActions.Count; i++)
                {
                    if (groundedActions[i] is ActionDecl checkAct)
                    {
                        if (state.IsNodeTrue(checkAct.Preconditions))
                        {
                            var newState = state.Copy();
                            newState.ExecuteNode(checkAct.Effects);
                            evaluated[i] = h.GetValue(newState);
                        }
                    }
                }

                var best = evaluated[evaluated.ToList().IndexOf(evaluated.Max())];
                if (groundedActions[best] is ActionDecl act)
                    state.ExecuteNode(act.Effects);
            }
            return new ActionPlan(actionSteps, actionSteps.Count);
        }
    }
}
