using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    var newNode = new OrExp(derived.Parent);
                    foreach (var derivedDecl in derived.GetDecls())
                    {
                        var declCopy = derivedDecl.Copy();
                        for (int i = 0; i < derived.Arguments.Count; i++)
                        {
                            var allRefs = declCopy.FindNames(derived.Arguments[i].Name);
                            foreach (var refItem in allRefs)
                                refItem.Name = derived.Arguments[i].Name;
                        }
                        newNode.Options.Add(declCopy.Expression);
                    }

                    walk.Replace(derived, newNode);
                }
                else
                    throw new Exception("Parent for derived deconstruction must be a IWalkable!");
            }

            return (T)copy;
        }
    }
}
