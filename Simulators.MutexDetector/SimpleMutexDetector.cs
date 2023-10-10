using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.MutexDetector
{
    public class SimpleMutexDetector : IMutexDetectors
    {
        public List<PredicateExp> FindMutexes(PDDLDecl decl)
        {
            List<PredicateExp> mutexCandidates = new List<PredicateExp>();
            if (decl.Domain.Predicates != null)
                foreach (var predicate in decl.Domain.Predicates.Predicates)
                    mutexCandidates.Add(predicate.Copy(null));

            foreach(var act in decl.Domain.Actions)
            {

            }

            return mutexCandidates;
        }
    }
}
