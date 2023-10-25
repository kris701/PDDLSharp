using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;

namespace PDDLSharp.Toolkit.Grounders
{
    public class ParametizedGrounder : BaseGrounder<IParametized>
    {
        public ParametizedGrounder(PDDLDecl declaration) : base(declaration)
        {
        }

        public override List<IParametized> Ground(IParametized item)
        {
            List<IParametized> groundedParametizeds = new List<IParametized>();

            if (item.Parameters.Values.Count == 0 && item.Copy() is IParametized newItem)
                return new List<IParametized>() { newItem };

            var allPermuations = GenerateParameterPermutations(item.Parameters.Values);
            foreach (var permutation in allPermuations)
            {
                var copy = item.Copy();

                for (int i = 0; i < item.Parameters.Values.Count; i++)
                {
                    var allRefs = copy.FindNames(item.Parameters.Values[i].Name);
                    foreach (var refItem in allRefs)
                        refItem.Name = GetObjectFromIndex(permutation[i]);
                }

                if (copy is IParametized param)
                    groundedParametizeds.Add(param);
            }

            return groundedParametizeds;
        }
    }
}
