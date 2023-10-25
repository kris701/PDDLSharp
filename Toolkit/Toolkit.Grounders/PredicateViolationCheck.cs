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
        public int[] Indexes { get; }

        public PredicateViolationCheck(PredicateExp predicate, int[] indexes)
        {
            Predicate = predicate;
            Indexes = indexes;
        }
    }
}
