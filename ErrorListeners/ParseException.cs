using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ErrorListeners
{
    public class ParseException : Exception
    {
        public List<ParseError> Errors { get; internal set; }
        public ParseException(List<ParseError> errors) : base(GenerateErrorString(errors))
        {
            Errors = errors;
        }

        private static string GenerateErrorString(List<ParseError> errors)
        {
            var msgStr = "";
            foreach (var error in errors)
                msgStr += $"{error}{Environment.NewLine}";
            return msgStr;
        }
    }
}
