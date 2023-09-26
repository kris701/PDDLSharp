using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers
{
    public interface IPDDLParser
    {
        IErrorListener Listener { get; }

        PDDLDecl Parse(string domainFile, string problemFile);
        DomainDecl ParseDomain(string domainFile);
        ProblemDecl ParseProblem(string problemFile);
    }
}
