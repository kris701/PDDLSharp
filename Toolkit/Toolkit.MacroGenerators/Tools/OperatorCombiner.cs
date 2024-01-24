using PDDLSharp.Models.SAS;
using PDDLSharp.Tools;

namespace PDDLSharp.Toolkit.MacroGenerators.Tools
{
    public class OperatorCombiner
    {
        public Operator Combine(List<Operator> operators)
        {
            var pre = new HashSet<Fact>();
            var add = new HashSet<Fact>();
            var del = new HashSet<Fact>();

            pre.AddRange(operators[0].Pre.ToHashSet());
            add.AddRange(operators[0].Add.ToHashSet());
            del.AddRange(operators[0].Del.ToHashSet());

            foreach (var op in operators.Skip(1))
            {
                foreach (var precon in op.Pre)
                    if (!add.Contains(precon))
                        pre.Add(precon);

                foreach (var delete in op.Del)
                    if (add.Contains(delete))
                        add.Remove(delete);

                foreach (var adding in op.Add)
                    if (del.Contains(adding))
                        del.Remove(adding);

                foreach (var delete in op.Del)
                    del.Add(delete);

                foreach (var adding in op.Add)
                    if (!pre.Contains(adding))
                        add.Add(adding);
            }

            return new Operator("macro", new string[0], pre.ToArray(), add.ToArray(), del.ToArray());
        }
    }
}
