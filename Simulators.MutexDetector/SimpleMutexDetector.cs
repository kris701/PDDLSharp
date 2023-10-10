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

            foreach(var act in decl.Domain.Actions)
            {
                Dictionary<PredicateExp, int> balance = new Dictionary<PredicateExp, int>();

                var items = act.Preconditions.FindTypes<PredicateExp>();
                items.AddRange(act.Effects.FindTypes<PredicateExp>());
                foreach (var precondition in items)
                {
                    var simplified = SimplifyPredicate(precondition);
                    if (!balance.ContainsKey(simplified))
                        balance.Add(simplified, 0);

                    if (precondition.Parent is NotExp)
                        balance[simplified]--;
                    else
                        balance[simplified]++;
                }

                var names = new List<string>();
                foreach (var key in balance.Keys)
                    if (!names.Contains(key.Name))
                        names.Add(key.Name);

                foreach(var name in names)
                {
                    bool isGood = true;
                    foreach (var key in balance.Keys)
                    {
                        if (key.Name == name && balance[key] != 0)
                        {
                            isGood = false;
                            break;
                        }
                    }

                    if (isGood)
                    {
                        if (decl.Domain.Predicates != null)
                            mutexCandidates.Add(decl.Domain.Predicates.Predicates.First(x => x.Name == name));
                        else
                        mutexCandidates.Add(new PredicateExp(name));
                    }
                }
            }

            return mutexCandidates;
        }

        private PredicateExp SimplifyPredicate(PredicateExp pred)
        {
            var newPred = new PredicateExp(pred.Name);
            foreach (var arg in pred.Arguments)
                newPred.Arguments.Add(new NameExp(arg.Name));
            return newPred;
        }
    }
}
