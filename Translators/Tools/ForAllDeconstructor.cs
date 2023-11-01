using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tools
{
    public class ForAllDeconstructor
    {
        public IGrounder<IParametized> Grounder { get; }
        public bool Aborted { get; set; } = false;

        public ForAllDeconstructor(IGrounder<IParametized> grounder)
        {
            Grounder = grounder;
        }

        public T DeconstructForAlls<T>(T node) where T : INode
        {
            var copy = node.Copy();
            var forAlls = copy.FindTypes<ForAllExp>();
            foreach(var forAll in forAlls)
            {
                if (forAll.Parent is IWalkable walk)
                {
                    var newNode = new AndExp(forAll.Parent);

                    var result = Grounder.Ground(forAll).Cast<ForAllExp>();
                    foreach (var item in result)
                        newNode.Add(item.Expression);

                    walk.Replace(forAll, newNode);
                }
                else
                    throw new Exception("Parent for forall deconstruction must be a IWalkable!");
            }

            return (T)copy;
        }
    }
}
