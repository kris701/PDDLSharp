using PDDLSharp.ErrorListeners;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PDDLSharp.Analysers
{
    public class GeneralPreAnalyser : IAnalyser<string>
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
            CheckForUnsupportedRequirements(text);
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

        public static List<string> UnsupportedPackages = new List<string>()
        {
            ":existential-preconditions",
            ":adl",
            ":universal-preconditions",
            ":quantified-preconditions",
            ":conditional-effects",
            ":action-expansions",
            ":foreach-expansions",
            ":dag-expansions",
            ":subgoals-through-axioms",
            ":safety-constraints",
            ":expression-evaluation",
            ":fluents",
            ":open-world",
            ":true-negation",
            ":ucpop"
        };
        private void CheckForUnsupportedRequirements(string text)
        {
            foreach(var unsuportedPackage in UnsupportedPackages)
            {
                if (text.Contains(unsuportedPackage))
                {
                    Listener.AddError(new ParseError(
                        $"The reqirement '{unsuportedPackage}' is not supported by this parser. Results may not be accurate!",
                        ParseErrorType.Warning,
                        ParseErrorLevel.PreParsing));
                }
            }
        }
    }
}
