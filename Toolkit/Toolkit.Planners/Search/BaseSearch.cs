using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.Plans;
using PDDLSharp.Models;
using PDDLSharp.Toolkit.Grounders;
using PDDLSharp.Toolkit.StateSpace;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Planners.Search
{
    public abstract class BaseSearch : IPlanner
    {
        public PDDLDecl Declaration { get; }
        public HashSet<ActionDecl> GroundedActions { get; set; }
        public int Generated { get; internal set; }
        public int Expanded { get; internal set; }

        private bool _preprocessed = false;

        public BaseSearch(PDDLDecl decl)
        {
            Declaration = decl;
            GroundedActions = new HashSet<ActionDecl>();
        }

        public void PreProcess()
        {
            if (_preprocessed)
                return;
            var grounder = new ParametizedGrounder(Declaration);
            GroundedActions = new HashSet<ActionDecl>();
            foreach (var action in Declaration.Domain.Actions)
                GroundedActions.AddRange(grounder.Ground(action).Cast<ActionDecl>().ToHashSet());
            _preprocessed = true;
        }

        public ActionPlan Solve(IHeuristic h)
        {
            IState state = new PDDLStateSpace(Declaration);
            return Solve(h, state);
        }

        public abstract ActionPlan Solve(IHeuristic h, IState state);
    }
}
