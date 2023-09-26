using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.Domain;
using PDDL.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Parsers
{
    public interface IPDDLParser
    {
        IErrorListener Listener { get; }

        PDDLDecl Parse(string domainFile, string problemFile);
        DomainDecl ParseDomain(string domainFile);
        ProblemDecl ParseProblem(string problemFile);
    }
}
