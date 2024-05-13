using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using System.Text;

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
                if (lastP == -1)
                    Listener.AddError(new PDDLSharpError($"Node started with a '(' but didnt end with one!: {text}", ParseErrorType.Error, ParseErrorLevel.PreParsing));
                var excludeSlices = new List<int>();

                var children = new List<ASTNode>();
                int offset = firstP;
                var innerLineOffset = lineOffset;
                while (text.IndexOf('(', offset + 1) != -1)
                {
                    int currentLevel = 0;
                    int startP = text.IndexOf('(', offset + 1);
                    int endP = text.Length;
                    for (int i = startP + 1; i < text.Length; i++)
                    {
                        if (text[i] == '(')
                            currentLevel++;
                        else if (text[i] == ')')
                        {
                            if (currentLevel == 0)
                            {
                                endP = i + 1;
                                break;
                            }
                            currentLevel--;
                        }
                    }

                    offset = endP - 1;
                    excludeSlices.Add(startP);
                    excludeSlices.Add(endP);
                    var newContent = text.Substring(startP, endP - startP);
                    innerLineOffset = GetLineNumber(lineDict, startP, innerLineOffset) - 1;
                    children.Add(GenerateASTNodeRec(newContent, thisStart + startP, thisStart + endP, lineDict, innerLineOffset - 1));
                }
                var newInnerContent = GenerateInnerContent(text, excludeSlices);
                firstP = newInnerContent.IndexOf('(');
                lastP = newInnerContent.LastIndexOf(')');
                newInnerContent = newInnerContent.Substring(firstP + 1, lastP - firstP - 1);

                return new ASTNode(
                    lineOffset,
                    $"({newInnerContent})",
                    newInnerContent,
                    children);
            }
            else
            {
                var newText = text.Trim();
                return new ASTNode(
                    lineOffset,
                    newText,
                    newText);
            }
        }

        private string GenerateInnerContent(string innerContent, List<int> slices)
        {
            if (slices.Count == 0)
                return innerContent;
            var sb = new StringBuilder();

            slices = slices.Order().ToList();

            var from = 0;
            for (int i = 0; i < slices.Count; i += 2)
            {
                sb.Append(innerContent.Substring(from, slices[i] - from));
                from = slices[i + 1];
            }
            sb.Append(innerContent.Substring(from));

            return sb.ToString();
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
