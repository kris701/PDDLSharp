using PDDL.Models.Domain;
using PDDL.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PDDL.Tools;

namespace PDDL.Models
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
    }
}
