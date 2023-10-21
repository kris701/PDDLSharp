using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;

namespace PDDLSharp.ASTGenerators.SAS
{
    public class SASASTGenerator : BaseASTGenerator
    {
        public SASASTGenerator(IErrorListener listener) : base(listener)
        {
        }

        public override ASTNode Generate(string text)
        {
            text = SASTextPreprocessing.ReplaceSpecialCharacters(text);
            text = SASTextPreprocessing.TokenizeSpecials(text);

            var lineDict = GenerateLineDict(text, SASASTTokens.BreakToken);

            var returnNode = new ASTNode(0, text.Length, 1, text, text);
            int offset = 0;
            int lineOffset = 0;
            while (offset != -1)
            {
                // Find indexes of the next two 'begin_' and 'end_' sections
                var begin = text.IndexOf('?', offset);
                offset = text.IndexOf('?', begin + 1);
                // If either of them was not fount, then we are at the end of the file
                if (offset == -1 || begin == -1)
                    break;
                // Set the 'begin?' 5 characters back, to the start of the section
                begin -= 5;

                // Get the last break token before the section end.
                var lastBreak = text.IndexOf(SASASTTokens.BreakToken, offset);
                var endLength = text.Substring(offset, lastBreak - offset).Length;
                // Using the 'begin' and the 'offset' + 'endLength' get a substring thats the outer content of the section
                var outerText = text.Substring(begin, offset - begin + endLength).Replace(SASASTTokens.BeginToken, "begin_").Replace(SASASTTokens.EndToken, "end_");
                string innerText = "";
                // If there are something inside the outer content, isolate it as the inner content
                if (outerText.IndexOf(SASASTTokens.BreakToken) != outerText.LastIndexOf(SASASTTokens.BreakToken))
                    innerText = outerText.Substring(outerText.IndexOf(SASASTTokens.BreakToken) + 1, outerText.LastIndexOf(SASASTTokens.BreakToken) - outerText.IndexOf(SASASTTokens.BreakToken) - 1);
                // Generate line number, based on the begin value
                lineOffset = GetLineNumber(lineDict, begin, lineOffset);
                returnNode.Children.Add(new ASTNode(
                    begin + 1,
                    offset + endLength,
                    lineOffset,
                    outerText,
                    innerText
                    ));
                offset++;
            }
            return returnNode;
        }
    }
}
