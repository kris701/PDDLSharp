using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Tools
{
    public class ImplyDeconstructor
    {
        public bool Aborted { get; set; } = false;

        public T DeconstructImplies<T>(T node) where T : INode
        {
            var copy = node.Copy();
            var implies = copy.FindTypes<ImplyExp>();
            foreach (var imply in implies)
            {
                if (Aborted) break;
                if (imply.Parent is IWalkable walk)
                {
                    var newNode = new OrExp(imply.Parent);
                    newNode.Options.Add(new NotExp(newNode, imply.Antecedent));
                    newNode.Options.Add(new AndExp(newNode, new List<IExp>() {
                        imply.Antecedent,
                        imply.Consequent
                    }));

                    walk.Replace(imply, newNode);
                }
                else
                    throw new Exception("Parent for imply deconstruction must be a IWalkable!");
            }

            return (T)copy;
        }
    }
}
