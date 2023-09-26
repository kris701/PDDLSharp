using PDDLModels.Domain;
using PDDLModels.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tools;

namespace PDDLModels
{
    public class PDDLDecl
    {
        public DomainDecl Domain { get; set; }
        public ProblemDecl Problem { get; set; }

        public PDDLDecl(DomainDecl domain, ProblemDecl problem)
        {
            Domain = domain;
            Problem = problem;
        }

        public HashSet<INamedNode> FindNames(string name)
        {
            var matches = new HashSet<INamedNode>();
            if (Domain != null)
                matches.AddRange(Domain.FindNames(name));
            if (Problem != null)
                matches.AddRange(Problem.FindNames(name));
            return matches;
        }

        public HashSet<T> FindTypes<T>()
        {
            var matches = new HashSet<T>();
            if (Domain != null)
                matches.AddRange(Domain.FindTypes<T>());
            if (Problem != null)
                matches.AddRange(Problem.FindTypes<T>());
            return matches;
        }
    }
}
