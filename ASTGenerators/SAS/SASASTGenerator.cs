using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators.SAS
{
    public class SASASTGenerator : IGenerator
    {
        public IErrorListener Listener { get; }

        public SASASTGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public ASTNode Generate(FileInfo file) => Generate(File.ReadAllText(file.FullName));

        public ASTNode Generate(string text)
        {
            text = SASTextPreprocessing.ReplaceSpecialCharacters(text);

            var returnNode = new ASTNode(0, text.Length, text, text);
            int offset = 0;
            while (offset != -1)
            {
                var begin = text.IndexOf("begin_", offset);
                offset = text.IndexOf("end_", offset + 1);
                if (offset == -1)
                    break;
                var lastBreak = text.IndexOf(SASASTTokens.BreakToken, offset);
                var endLength = text.Substring(offset, lastBreak - offset).Length;
                var outerText = text.Substring(begin, offset - begin + endLength).Trim();
                var innerText = outerText.Substring(outerText.IndexOf(SASASTTokens.BreakToken) + 1, outerText.LastIndexOf(SASASTTokens.BreakToken) - outerText.IndexOf(SASASTTokens.BreakToken) - 1).Trim();
                returnNode.Children.Add(new ASTNode(
                    begin,
                    offset,
                    outerText,
                    innerText
                    ));
            }
            return returnNode;
        }
    }
}
