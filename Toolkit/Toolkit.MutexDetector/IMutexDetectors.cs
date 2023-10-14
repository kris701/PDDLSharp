using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Toolkit.MutexDetector
{
    public interface IMutexDetectors
    {
        public List<PredicateExp> FindMutexes(PDDLDecl decl);
    }
}
