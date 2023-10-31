using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.SAS;

namespace PDDLSharp.Toolkit.StateSpace.SAS
{
    public class RelaxedSASStateSpace : SASStateSpace
    {
        public RelaxedSASStateSpace(SASDecl declaration) : base(declaration)
        {
        }

        public RelaxedSASStateSpace(SASDecl declaration, HashSet<Fact> state) : base(declaration, state)
        {
        }

        public override int ExecuteNode(Operator node)
        {
            int changes = 0;
            //foreach (var fact in node.Del)
            //    if (State.Remove(fact))
            //        changes--;
            foreach (var fact in node.Add)
                if (State.Add(fact))
                    changes++;
            return changes;
        }

        public override ISASState Copy()
        {
            var newState = new Fact[State.Count];
            State.CopyTo(newState);
            return new RelaxedSASStateSpace(Declaration, newState.ToHashSet());
        }
    }
}
