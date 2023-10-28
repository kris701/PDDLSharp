using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Toolkit.Grounders
{
    internal class PredicateViolationCheck
    {
        public PredicateExp Predicate { get; }
        public int[] ArgIndexes { get; }
        public int[] ConstantsIndexes { get; }

        public PredicateViolationCheck(PredicateExp predicate, int[] argIndexes, int[] constantsIndexes)
        {
            Predicate = predicate;
            ArgIndexes = argIndexes;
            ConstantsIndexes = constantsIndexes;
        }
    }
}
