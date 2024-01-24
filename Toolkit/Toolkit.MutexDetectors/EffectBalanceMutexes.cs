using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.MutexDetectors
{
    public class EffectBalanceMutexes : IMutexDetectors
    {
        public List<PredicateExp> FindMutexes(PDDLDecl decl)
        {
            List<PredicateExp> mutexCandidates = new List<PredicateExp>();
            List<PredicateExp> notMutexCandidates = new List<PredicateExp>();

            foreach (var act in decl.Domain.Actions)
            {
                Dictionary<string, int> balance = new Dictionary<string, int>();

                var items = act.Effects.FindTypes<PredicateExp>();
                foreach (var precondition in items)
                {
                    if (!balance.ContainsKey(precondition.Name))
                        balance.Add(precondition.Name, 0);

                    if (precondition.Parent is NotExp)
                        balance[precondition.Name]--;
                    else
                        balance[precondition.Name]++;
                }

                foreach (var name in balance.Keys)
                {
                    if (balance[name] == 0 && !notMutexCandidates.Any(x => x.Name == name))
                    {
                        if (decl.Domain.Predicates != null)
                            mutexCandidates.Add(decl.Domain.Predicates.Predicates.First(x => x.Name == name));
                        else
                            mutexCandidates.Add(new PredicateExp(name));
                    }
                    else
                    {
                        notMutexCandidates.Add(new PredicateExp(name));
                        mutexCandidates.RemoveAll(x => x.Name == name);
                    }
                }
            }

            return mutexCandidates;
        }
    }
}
