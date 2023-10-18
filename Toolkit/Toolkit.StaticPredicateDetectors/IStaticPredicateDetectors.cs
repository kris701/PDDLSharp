using PDDLSharp.Models;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.StaticPredicateDetectors
{
    public interface IStaticPredicateDetectors
    {
        public List<PredicateExp> FindStaticPredicates(PDDLDecl decl);
    }
}
