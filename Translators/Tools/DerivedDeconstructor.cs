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
            var copy = node.Copy(node.Parent);
            var derivedExps = copy.FindTypes<DerivedPredicateExp>();
            while(derivedExps.Count > 0)
            {
                if (Aborted) break;
                if (derivedExps[0].Parent is IWalkable walk)
                {
                    var derivedDecls = derivedExps[0].GetDecls();
                    if (derivedDecls.Count > 1)
                        throw new TranslatorException("Translator does not support derived predicates with multiple declarations!");
                    if (derivedDecls.Count == 0)
                        throw new Exception("Derived predicate did not have any declaration in the domain?");

                    var declCopy = derivedDecls[0].Copy(derivedExps[0].Parent);
                    for (int i = 0; i < derivedExps[0].Arguments.Count; i++)
                    {
                        var allRefs = declCopy.Expression.FindNames(declCopy.Predicate.Arguments[i].Name);
                        foreach (var refItem in allRefs)
                            refItem.Name = derivedExps[0].Arguments[i].Name;
                    }
                    walk.Replace(derivedExps[0], declCopy.Expression);
                }
                else
                    throw new Exception("Parent for derived deconstruction must be a IWalkable!");
                derivedExps = copy.FindTypes<DerivedPredicateExp>();
            }

            return (T)copy;
        }
    }
}
