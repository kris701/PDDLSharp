using PDDLSharp.ErrorListeners;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PDDLSharp.Analysers
{
    public class GeneralPreAnalyser
    {
        public IErrorListener Listener { get; }

        public GeneralPreAnalyser(IErrorListener listener)
        {
            Listener = listener;
        }

        public void PostAnalyse(string decl)
        {
            throw new NotImplementedException();
        }

        public void PreAnalyse(string text)
        {
            CheckParenthesesMissmatch(text);
            CheckForCasing(text);
            if (!CompatabilityHelper.IsPDDLDomainSpported(text))
                Listener.AddError(new ParseError(
                    $"Domain contains unsupported packages! Results may not be accurate!",
                    ParseErrorType.Warning,
                    ParseErrorLevel.PreParsing));
        }

        private void CheckParenthesesMissmatch(string text)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                Listener.AddError(new ParseError(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));
            }
        }

        private void CheckForCasing(string text)
        {
            if (text.Any(char.IsUpper))
            {
                Listener.AddError(new ParseError(
                    $"Upper cased letters are ignored in PDDL",
                    ParseErrorType.Message,
                    ParseErrorLevel.PreParsing));
            }
        }
    }
}
