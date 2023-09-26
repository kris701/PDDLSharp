using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErrorListeners
{
    public class ErrorListener : IErrorListener
    {
        public ParseErrorType ThrowIfTypeAbove { get; set; }
        public List<ParseError> Errors { get; internal set; }

        public ErrorListener()
        {
            Errors = new List<ParseError>();
            ThrowIfTypeAbove = ParseErrorType.None;
        }

        public void AddError(ParseError err)
        {
            Errors.Add(err);
            if (Errors.Any(x => x.Type > ThrowIfTypeAbove))
                throw new ParseException(Errors);
        }
    }
}
