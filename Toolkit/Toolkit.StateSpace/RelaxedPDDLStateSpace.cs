using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Toolkit.Grounders;

namespace PDDLSharp.Toolkit.StateSpace
{
    public class RelaxedPDDLStateSpace : PDDLStateSpace
    {
        public RelaxedPDDLStateSpace(PDDLDecl declaration) : base(declaration)
        {
        }

        public RelaxedPDDLStateSpace(PDDLDecl declaration, HashSet<PredicateExp> currentState) : base(declaration, currentState)
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

        public override IState Copy()
        {
            PredicateExp[] newState = new PredicateExp[State.Count];
            State.CopyTo(newState);
            return new RelaxedPDDLStateSpace(Declaration, newState.ToHashSet());
        }
    }
}
