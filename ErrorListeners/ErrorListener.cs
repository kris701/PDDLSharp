using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ErrorListeners
{
    public class ErrorListener : IErrorListener
    {
        public ParseErrorType ThrowIfTypeAbove { get; set; }
        public List<PDDLSharpError> Errors { get; internal set; }

        public ErrorListener()
        {
            Errors = new List<PDDLSharpError>();
            ThrowIfTypeAbove = ParseErrorType.Warning;
        }

        public ErrorListener(ParseErrorType throwAbove)
        {
            Errors = new List<PDDLSharpError>();
            ThrowIfTypeAbove = throwAbove;
        }

        public void AddError(PDDLSharpError err)
        {
            Errors.Add(err);
            if (err.Type > ThrowIfTypeAbove)
                throw new PDDLSharpException(Errors);
        }

        public int CountErrorsOfTypeOrAbove(ParseErrorType type)
        {
            int count = 0;
            foreach(var error in Errors)
                if (error.Type >= type)
                    count++;
            return count;
        }
    }
}
