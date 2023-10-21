using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.StaticPredicateDetectors
{
    public interface IStaticPredicateDetectors
    {
        public List<PredicateExp> FindStaticPredicates(PDDLDecl decl);
    }
}
