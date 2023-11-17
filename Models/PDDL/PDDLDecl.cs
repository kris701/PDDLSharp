using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Problem;
using System.Collections;

namespace PDDLSharp.Models.PDDL
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

        public PDDLDecl()
        {
            Domain = new DomainDecl();
            Problem = new ProblemDecl();
        }

        public override bool Equals(object? obj)
        {
            if (obj is PDDLDecl other)
            {
                if (!Domain.Equals(other.Domain)) return false;
                if (!Problem.Equals(other.Problem)) return false;
                return true;
            }
            return false;
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
                Domain.Copy(),
                Problem.Copy()
                );
        }
    }
}
