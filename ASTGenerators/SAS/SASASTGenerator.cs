using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var begin = text.IndexOf('?', offset);
                offset = text.IndexOf('?', begin + 1);
                if (offset == -1 || begin == -1)
                    break;
                begin -= 5;
                var lastBreak = text.IndexOf(SASASTTokens.BreakToken, offset);
                var endLength = text.Substring(offset, lastBreak - offset).Length;
                var outerText = text.Substring(begin, offset - begin + endLength);
                string innerText = "";
                if (outerText.Count(x => x == SASASTTokens.BreakToken) > 1)
                    innerText = outerText.Substring(outerText.IndexOf(SASASTTokens.BreakToken) + 1, outerText.LastIndexOf(SASASTTokens.BreakToken) - outerText.IndexOf(SASASTTokens.BreakToken) - 1);
                lineOffset = GetLineNumber(lineDict, begin, lineOffset);
                returnNode.Children.Add(new ASTNode(
                    begin + 1,
                    offset + endLength,
                    lineOffset,
                    outerText.Replace("begin?", "begin_").Replace("end?", "end_"),
                    innerText.Replace("begin?", "begin_").Replace("end?", "end_")
                    ));
                offset++;
            }
            return returnNode;
        }

    }
}
