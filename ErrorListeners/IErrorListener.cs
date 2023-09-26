using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.ErrorListeners
{
    public interface IErrorListener
    {
        ParseErrorType ThrowIfTypeAbove { get; set; }
        List<ParseError> Errors { get; }
        void AddError(ParseError err);
    }
}
