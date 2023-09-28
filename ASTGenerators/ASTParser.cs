﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators
{
    public class ASTParser : IASTParser<ASTNode>
    {
        public ASTNode Parse(string text)
        {
            if (File.Exists(text))
                text = File.ReadAllText(text);

            text = TokenizeSpecials(text);

            int end = text.Length;
            if (text.Contains(')'))
                end = text.LastIndexOf(')') + 1;

            var node = ParseAsNodeRec(text, 0, end);
            SetLineNumbersByCharacter(text, node);
            return node;
        }

        public string TokenizeSpecials(string text)
        {
            text = text.Replace("\n- ", $"\n{ASTTokens.TypeToken}");
            text = text.Replace(" - ", ASTTokens.TypeToken);
            return text;
        }

        private ASTNode ParseAsNodeRec(string text, int thisStart, int thisEnd)
        {
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
                    children.Add(ParseAsNodeRec(newContent, thisStart + startP, thisStart + endP));
                    innerContent = ReplaceRangeWithSpaces(innerContent, startP, endP);
                }
                var outer = $"({innerContent.Trim()})";
                return new ASTNode(
                    thisStart,
                    thisEnd,
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
                    newText,
                    newText);
            }
        }

        private string ReplaceRangeWithSpaces(string text, int from, int to)
        {
            return text.Remove(from, to - from).Insert(from, new string(' ', to - from));
        }

        private void SetLineNumbersByCharacter(string source, ASTNode node)
        {
            List<int> lineDict = new List<int>();
            int offset = source.IndexOf(ASTTokens.BreakToken);
            while(offset != -1)
            {
                lineDict.Add(offset);
                offset = source.IndexOf(ASTTokens.BreakToken, offset + 1);
            }

            SetLineNumberByCharacterNumberRec(lineDict, node);
        }

        private void SetLineNumberByCharacterNumberRec(List<int> lineDict, ASTNode node)
        {
            foreach (var child in node.Children)
                SetLineNumberByCharacterNumberRec(lineDict, child);

            int counter = 1;
            foreach(var endChar in lineDict)
            {
                if (endChar > node.Start)
                {
                    node.Line = counter;
                    break;
                }
                counter++;
            }
            if (node.Line == -1)
                node.Line = lineDict.Count + 1;
        }
    }
}
