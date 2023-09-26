using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDLSharp.Tools;

namespace PDDLSharp.Models
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

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj is not PDDLDecl)
                return false;
            var hash1 = obj.GetHashCode();
            var hash2 = GetHashCode();
            return hash1 == hash2;
        }

        public override int GetHashCode()
        {
            return Domain.GetHashCode() ^ Problem.GetHashCode();
        }
    }
}
