using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators
{
    public class ASTGenerator : IGenerator<ASTNode>
    {
        public ASTNode Generate(string text)
        {
            if (File.Exists(text))
                text = File.ReadAllText(text);

            text = TokenizeSpecials(text);

            int end = text.Length;
            if (text.Contains(')'))
                end = text.LastIndexOf(')') + 1;

            var lineDict = GenerateLineDict(text);
            var node = ParseAsNodeRec(text, 0, end, lineDict, 0);
            return node;
        }

        public string TokenizeSpecials(string text)
        {
            text = text.Replace("\n- ", $"\n{ASTTokens.TypeToken}");
            text = text.Replace(" - ", ASTTokens.TypeToken);
            return text;
        }

        private ASTNode ParseAsNodeRec(string text, int thisStart, int thisEnd, List<int> lineDict, int lineOffset)
        {
            lineOffset = GetLineNumber(lineDict, thisStart, lineOffset);
            if (text.Contains('('))
            {
                var firstP = text.IndexOf('(');
                var lastP = text.LastIndexOf(')');
                var innerContent = ReplaceRangeWithSpaces(text, firstP, firstP + 1);
                innerContent = ReplaceRangeWithSpaces(innerContent, lastP, lastP + 1);

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
                    children.Add(ParseAsNodeRec(newContent, thisStart + startP, thisStart + endP, lineDict, lineOffset - 1));
                    innerContent = ReplaceRangeWithSpaces(innerContent, startP, endP);
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

        // Faster string replacement
        // From https://stackoverflow.com/a/54056154
        private string ReplaceRangeWithSpaces(string text, int from, int to)
        {
            int length = to - from;
            string replacement = new string(' ', to - from);
            return string.Create(text.Length - length + replacement.Length, (text, from, length, replacement),
                (span, state) =>
                {
                    state.text.AsSpan().Slice(0, state.from).CopyTo(span);
                    state.replacement.AsSpan().CopyTo(span.Slice(state.from));
                    state.text.AsSpan().Slice(state.from + state.length).CopyTo(span.Slice(state.from + state.replacement.Length));
                });
        }

        private List<int> GenerateLineDict(string source)
        {
            List<int> lineDict = new List<int>();
            int offset = source.IndexOf(ASTTokens.BreakToken);
            while (offset != -1)
            {
                lineDict.Add(offset);
                offset = source.IndexOf(ASTTokens.BreakToken, offset + 1);
            }
            return lineDict;
        }

        private int GetLineNumber(List<int> lineDict, int start, int offset)
        {
            int length = lineDict.Count;
            for (int i = offset; i < length; i++)
                if (start < lineDict[i])
                    return i + 1;
            return lineDict.Count + 1;
        }
    }
}
