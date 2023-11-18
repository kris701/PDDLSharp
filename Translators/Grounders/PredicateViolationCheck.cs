using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Translators.Grounders
{
    internal class PredicateViolationCheck
    {
        public bool IsTrue { get; }
        public PredicateExp Predicate { get; }
        public int[] ArgIndexes { get; }
        public int[] ConstantsIndexes { get; }

        public PredicateViolationCheck(PredicateExp predicate, int[] argIndexes, int[] constantsIndexes, bool isTrue)
        {
            Predicate = predicate;
            ArgIndexes = argIndexes;
            ConstantsIndexes = constantsIndexes;
            IsTrue = isTrue;
        }
    }
}
