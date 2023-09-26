using ErrorListeners;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsers
{
    public interface IPDDLParser
    {
        IErrorListener Listener { get; }
        bool Contextualise { get; set; }
        bool Analyse { get; set; }

        PDDLDecl Parse(string domainFile = null, string problemFile = null);
        PDDLDecl TryParse(string domainFile = null, string problemFile = null);
    }
}
