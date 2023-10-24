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
            IGrounder<ActionDecl> grounder = new ActionGrounder(new PDDLDecl(Domain, Problem));
            List<ActionDecl> groundedActions = new List<ActionDecl>();
            foreach (var action in Domain.Actions)
                groundedActions.AddRange(grounder.Ground(action));

            IState state = new RelaxedPDDLStateSpace(new PDDLDecl(Domain, Problem));
            List<GroundedAction> actionSteps = new List<GroundedAction>();
            HashSet<IState> openList = new HashSet<IState>();
            HashSet<IState> closedList = new HashSet<IState>();
            while (!state.IsInGoal())
            {
                int best = int.MaxValue;
                int bestAction = -1;
                for(int i = 0; i < groundedActions.Count; i++)
                {
                    if (state.IsNodeTrue(groundedActions[i].Preconditions))
                    {
                        var check = state.Copy();
                        check.ExecuteNode(groundedActions[i].Effects);
                        var value = h.GetValue(state, groundedActions[i]);
                        if (value <= best && !closedList.Contains(check))
                        {
                            closedList.Add(check);
                            state = check;
                            best = value;
                            bestAction = i;
                        }
                    }
                }

                actionSteps.Add(new GroundedAction(groundedActions[bestAction])); 
            }
            return new ActionPlan(actionSteps, actionSteps.Count);
        }
    }
}
