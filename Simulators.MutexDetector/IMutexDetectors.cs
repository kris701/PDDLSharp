using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.MutexDetector
{
    public interface IMutexDetectors
    {
        public List<PredicateExp> FindMutexes(PDDLDecl decl);
    }
}
