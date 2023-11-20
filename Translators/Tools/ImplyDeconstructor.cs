using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;
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

            while (implies.Count > 0)
            {
                if (Aborted) break;
                if (implies[0].Parent is IWalkable walk)
                {
                    var newNode = new OrExp(implies[0].Parent);
                    var notOption = new NotExp(newNode, new EmptyExp());
                    if (implies[0].Antecedent.Copy(notOption) is IExp nExp)
                        notOption.Child = nExp;
                    newNode.Options.Add(notOption);

                    var andOption = new AndExp(newNode, new List<IExp>());
                    if (implies[0].Antecedent.Copy(andOption) is IExp exp1)
                        andOption.Children.Add(exp1);
                    if (implies[0].Consequent.Copy(andOption) is IExp exp2)
                        andOption.Children.Add(exp2);
                    newNode.Options.Add(andOption);

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
