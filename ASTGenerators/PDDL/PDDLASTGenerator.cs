using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.ASTGenerators.PDDL
{
    public class PDDLASTGenerator : BaseASTGenerator
    {
        public PDDLASTGenerator(IErrorListener listener) : base(listener)
        {
        }

        public override ASTNode Generate(string text)
        {
            text = PDDLTextPreprocessing.ReplaceCommentsWithWhiteSpace(text);
            PreCheck(text);
            text = text.ToLower();
            text = PDDLTextPreprocessing.ReplaceSpecialCharacters(text);
            text = PDDLTextPreprocessing.TokenizeSpecials(text);

            int end = GetEndIndex(text);
            var lineDict = GenerateLineDict(text, PDDLASTTokens.BreakToken);
            var node = GenerateASTNodeRec(text, 0, end, lineDict, 0);
            return node;
        }

        public int GetEndIndex(string text)
        {
            int end = text.Length;
            if (text.Contains(')'))
                end = text.LastIndexOf(')') + 1;
            return end;
        }

        public ASTNode GenerateASTNodeRec(string text, int thisStart, int thisEnd, List<int> lineDict, int lineOffset)
        {
            lineOffset = GetLineNumber(lineDict, thisStart, lineOffset);
            if (text.Contains('('))
            {
                var firstP = text.IndexOf('(');
                var lastP = text.LastIndexOf(')');
                var innerContent = StringHelpers.ReplaceRangeWithSpacesFast(text, firstP, firstP + 1);
                innerContent = StringHelpers.ReplaceRangeWithSpacesFast(innerContent, lastP, lastP + 1);

                var children = new List<ASTNode>();
                while (innerContent.Contains('(') && innerContent.Contains(')'))
                {
                    int currentLevel = 0;
                    int startP = innerContent.IndexOf('(');
                    int endP = innerContent.Length;
                    for (int i = startP + 1; i < innerContent.Length; i++)
                    {
                        if (innerContent[i] == '(')
                            currentLevel++;
                        else if (innerContent[i] == ')')
                        {
                            if (currentLevel == 0)
                            {
                                endP = i + 1;
                                break;
                            }
                            currentLevel--;
                        }
                    }

                    var newContent = innerContent.Substring(startP, endP - startP);
                    children.Add(GenerateASTNodeRec(newContent, thisStart + startP, thisStart + endP, lineDict, lineOffset - 1));
                    innerContent = StringHelpers.ReplaceRangeWithSpacesFast(innerContent, startP, endP);
                }
                var outer = $"({innerContent.Trim()})";
                return new ASTNode(
                    thisStart,
                    thisEnd,
                    lineOffset,
                    outer,
                    innerContent.Trim(),
                    children);
            }
            else
            {
                var newText = text.Trim();
                return new ASTNode(
                    thisStart,
                    thisEnd,
                    lineOffset,
                    newText,
                    newText);
            }
        }

        private void PreCheck(string text)
        {
            CheckParenthesesMissmatch(text);
            CheckForCasing(text);
        }

        private void CheckParenthesesMissmatch(string text)
        {
            var leftCount = text.Count(x => x == '(');
            var rightCount = text.Count(x => x == ')');
            if (leftCount != rightCount)
            {
                Listener.AddError(new PDDLSharpError(
                    $"Parentheses missmatch! There are {leftCount} '(' but {rightCount} ')'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.PreParsing));
            }
        }

        private void CheckForCasing(string text)
        {
            if (text.Any(char.IsUpper))
            {
                Listener.AddError(new PDDLSharpError(
                    $"Upper cased letters are ignored in PDDL",
                    ParseErrorType.Message,
                    ParseErrorLevel.PreParsing));
            }
        }
    }
}
