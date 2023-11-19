using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Translators.Exceptions;
using PDDLSharp.Models.PDDL.Domain;

namespace PDDLSharp.Translators.Tools
{
    public class DerivedDeconstructor
    {
        public bool Aborted { get; set; } = false;

        public T DeconstructDeriveds<T>(T node) where T : INode
        {
            var copy = node.Copy();
            var derivedExps = copy.FindTypes<DerivedPredicateExp>();
            foreach (var derived in derivedExps)
            {
                if (Aborted) break;
                if (derived.Parent is IWalkable walk)
                {
                    var derivedDecls = derived.GetDecls();
                    if (derivedDecls.Count > 1)
                        throw new TranslatorException("Translator does not support derived predicates with multiple declarations!");
                    if (derivedDecls.Count == 0)
                        throw new Exception("Derived predicate did not have any declaration in the domain?");

                    var declCopy = derivedDecls[0].Copy();
                    for (int i = 0; i < derived.Arguments.Count; i++)
                    {
                        var allRefs = declCopy.Expression.FindNames(declCopy.Predicate.Arguments[i].Name);
                        foreach (var refItem in allRefs)
                            refItem.Name = derived.Arguments[i].Name;
                    }
                    walk.Replace(derived, declCopy.Expression);
                }
                else
                    throw new Exception("Parent for derived deconstruction must be a IWalkable!");
            }

            return (T)copy;
        }
    }
}
