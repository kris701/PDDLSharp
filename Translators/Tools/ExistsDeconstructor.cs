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
                    var result = Grounder.Ground(exists[0]).Cast<ExistsExp>().ToList();
                    if (result.Count == 1)
                    {
                        result[0].Expression.Parent = exists[0].Parent;
                        walk.Replace(exists[0], result[0].Expression);
                    }
                    else if (result.Count > 1)
                    {
                        var newNode = new OrExp(exists[0].Parent);
                        foreach (var item in result)
                        {
                            item.Expression.Parent = newNode;
                            newNode.Add(item.Expression);
                        }
                        walk.Replace(exists[0], newNode);
                    }
                }
                else
                    throw new Exception("Parent for exists deconstruction must be a IWalkable!");
                exists = copy.FindTypes<ExistsExp>();
            }

            return (T)copy;
        }
    }
}
