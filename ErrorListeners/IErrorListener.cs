using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ErrorListeners
{
    public interface IErrorListener
    {
        public ParseErrorType ThrowIfTypeAbove { get; set; }
        public List<ParseError> Errors { get; }
        public void AddError(ParseError err);
        public int CountErrorsOfTypeOrAbove(ParseErrorType type);
    }
}
