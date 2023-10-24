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

        public override void ExecuteNode(INode node)
        {
            _tempAdd.Clear();
            ExecuteNode(node, false);
            foreach (var item in _tempAdd)
                Add(item);
        }
    }
}
