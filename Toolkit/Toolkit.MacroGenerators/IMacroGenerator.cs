using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;

namespace PDDLSharp.Toolkit.MacroGenerators
{
    public interface IMacroGenerator<T>
    {
        public PDDLDecl Declaration { get; }
        public List<ActionDecl> FindMacros(T from, int amount = int.MaxValue);
    }
}
