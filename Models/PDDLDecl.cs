using Microsoft.VisualBasic;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;
using System;
using System.Collections;

namespace PDDLSharp.Models
{
    public class PDDLDecl : IEnumerable<INode>
    {
        public DomainDecl Domain { get; set; }
        public ProblemDecl Problem { get; set; }
        public bool IsContextualised { get; set; } = false;

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

        public IEnumerator<INode> GetEnumerator()
        {
            yield return Domain;
            yield return Problem;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public PDDLDecl Copy()
        {
            return new PDDLDecl(
                Domain.Copy(null),
                Problem.Copy(null)
                );
        }
    }
}
