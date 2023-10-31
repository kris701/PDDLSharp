using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Grounders;

namespace PDDLSharp.Toolkit.StateSpace.PDDL
{
    public class RelaxedPDDLStateSpace : PDDLStateSpace
    {
        public RelaxedPDDLStateSpace(PDDLDecl declaration) : base(declaration)
        {
        }

        public RelaxedPDDLStateSpace(PDDLDecl declaration, HashSet<PredicateExp> currentState, IGrounder<IParametized> grounder) : base(declaration, currentState, grounder)
        {
        }

        public override int ExecuteNode(INode node)
        {
            _tempAdd.Clear();
            ExecuteNode(node, false);
            int changes = 0;
            foreach (var item in _tempAdd)
                if (Add(item))
                    changes++;
            return changes;
        }

        public override IState<PredicateExp, INode> Copy()
        {
            PredicateExp[] newState = new PredicateExp[State.Count];
            State.CopyTo(newState);
            return new RelaxedPDDLStateSpace(Declaration, newState.ToHashSet(), _grounder);
        }
    }
}
