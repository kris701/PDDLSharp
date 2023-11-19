using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;

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
            var copy = node.Copy(node.Parent);
            var forAlls = copy.FindTypes<ForAllExp>();
            while(forAlls.Count > 0)
            {
                if (Aborted) break;
                if (forAlls[0].Parent is IWalkable walk)
                {
                    var newNode = new AndExp(forAlls[0].Parent);

                    var result = Grounder.Ground(forAlls[0]).Cast<ForAllExp>().ToList();
                    foreach (var item in result)
                        newNode.Add(item.Expression);

                    walk.Replace(forAlls[0], newNode);
                }
                else
                    throw new Exception("Parent for forall deconstruction must be a IWalkable!");
                forAlls = copy.FindTypes<ForAllExp>();
            }

            return (T)copy;
        }
    }
}
