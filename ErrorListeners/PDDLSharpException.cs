using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ErrorListeners
{
    public class PDDLSharpException : Exception
    {
        public List<PDDLSharpError> Errors { get; internal set; }
        public PDDLSharpException(List<PDDLSharpError> errors) : base(GenerateErrorString(errors))
        {
            Errors = errors;
        }

        private static string GenerateErrorString(List<PDDLSharpError> errors)
        {
            var msgStr = "";
            foreach (var error in errors)
                msgStr += $"{error}{Environment.NewLine}";
            return msgStr;
        }
    }
}
