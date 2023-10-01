using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PDDLSharp.ASTGenerators
{
    public class ASTGenerator : IGenerator
    {
        public IErrorListener Listener { get; }

        public ASTGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public ASTNode Generate(FileInfo file) => Generate(File.ReadAllText(file.FullName));

        public ASTNode Generate(string text)
        {
            PreCheck(text);
            text = text.ToLower();
            text = TextPreprocessing.ReplaceSpecialCharacters(text);
            text = TextPreprocessing.ReplaceCommentsWithWhiteSpace(text);
            text = TextPreprocessing.TokenizeSpecials(text);

            int end = GetEndIndex(text);
            var lineDict = GenerateLineDict(text);
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
                    children.Add(GenerateASTNodeRec(newContent, thisStart + startP, thisStart + endP, lineDict, lineOffset - 1));
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
        public string ReplaceRangeWithSpaces(string text, int from, int to)
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

        public List<int> GenerateLineDict(string source)
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

        public int GetLineNumber(List<int> lineDict, int start, int offset)
        {
            int length = lineDict.Count;
            for (int i = offset; i < length; i++)
                if (start < lineDict[i])
                    return i + 1;
            return lineDict.Count + 1;
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
