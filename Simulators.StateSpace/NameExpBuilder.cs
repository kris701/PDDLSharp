using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Simulators.StateSpace
{
    public static class NameExpBuilder
    {
        public static List<NameExp> GetNameExpFromString(string[] arguments, PDDLDecl decl)
        {
            var args = new List<NameExp>();
            foreach (var arg in arguments)
            {
                NameExp? obj = null;
                if (decl.Problem.Objects != null)
                    obj = decl.Problem.Objects.Objs.FirstOrDefault(x => x.Name == arg.ToLower());
                if (obj == null && decl.Domain.Constants != null)
                    obj = decl.Domain.Constants.Constants.FirstOrDefault(x => x.Name == arg.ToLower());

                if (obj == null)
                    throw new ArgumentException($"Cannot find object (or constant) '{arg}'");
                args.Add(new NameExp(obj));
            }
            return args;
        }
    }
}
