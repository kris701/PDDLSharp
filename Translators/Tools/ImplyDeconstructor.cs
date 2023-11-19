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
            var copy = node.Copy(node.Parent);
            var implies = copy.FindTypes<ImplyExp>();
            while(implies.Count > 0)
            {
                if (Aborted) break;
                if (implies[0].Parent is IWalkable walk)
                {
                    var newNode = new OrExp(implies[0].Parent);
                    newNode.Options.Add(new NotExp(newNode, implies[0].Antecedent));
                    newNode.Options.Add(new AndExp(newNode, new List<IExp>() {
                        implies[0].Antecedent,
                        implies[0].Consequent
                    }));

                    walk.Replace(implies[0], newNode);
                }
                else
                    throw new Exception("Parent for imply deconstruction must be a IWalkable!");
                implies = copy.FindTypes<ImplyExp>();
            }

            return (T)copy;
        }
    }
}
