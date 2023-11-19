using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Translators.Grounders;

namespace PDDLSharp.Translators.Tools
{
    public class ExistsDeconstructor
    {
        public IGrounder<IParametized> Grounder { get; }
        public bool Aborted { get; set; } = false;

        public ExistsDeconstructor(IGrounder<IParametized> grounder)
        {
            Grounder = grounder;
        }

        public T DeconstructExists<T>(T node) where T : INode
        {
            var copy = node.Copy(node.Parent);
            var exists = copy.FindTypes<ExistsExp>();
            while(exists.Count > 0)
            {
                if (Aborted) break;
                if (exists[0].Parent is IWalkable walk)
                {
                    var newNode = new OrExp(exists[0].Parent);

                    var result = Grounder.Ground(exists[0]).Cast<ExistsExp>();
                    foreach (var item in result)
                        newNode.Add(item.Expression);

                    walk.Replace(exists[0], newNode);
                }
                else
                    throw new Exception("Parent for exists deconstruction must be a IWalkable!");
                exists = copy.FindTypes<ExistsExp>();
            }

            return (T)copy;
        }
    }
}
