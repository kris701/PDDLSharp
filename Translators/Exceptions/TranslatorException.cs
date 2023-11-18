using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Translators.Exceptions
{
    public class TranslatorException : Exception
    {
        public TranslatorException(string? message) : base(message)
        {
        }
    }
}
