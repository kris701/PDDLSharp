using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Parsers.Visitors
{
    public abstract class BaseVisitor
    {
        internal static string ReplaceRangeWithSpaces(string text, int from, int to)
        {
            var newText = text.Substring(0, from);
            newText += new string(' ', to - from);
            newText += text.Substring(to);
            return newText;
        }

        internal static bool DoesNotContainStrayCharacters(ASTNode node, string targetName, IErrorListener listener)
        {
            if (node.InnerContent.Replace(targetName, "").Trim() != "")
            {
                listener.AddError(new ParseError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.OuterContent.Replace(targetName, "").Trim()}",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static bool DoesNodeHaveSpecificChildCount(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (targetChildren == 0)
            {
                if (node.Children.Count != 0)
                {
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must not contain any children!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
                    return false;
                }
            }
            else
            {
                if (node.Children.Count != targetChildren)
                {
                    listener.AddError(new ParseError(
                        $"'{nodeName}' must have exactly {targetChildren} children, but it has '{node.Children.Count}'!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
                    return false;
                }
            }
            return true;
        }

        internal static bool DoesNodeHaveMoreThanNChildren(ASTNode node, string nodeName, int targetChildren, IErrorListener listener)
        {
            if (node.Children.Count <= targetChildren)
            {
                listener.AddError(new ParseError(
                    $"'{nodeName}' must have more than {targetChildren} children, but it has '{node.Children.Count}'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static List<T> LooseParseString<T>(ASTNode node, INode parent, string nodeType, string content, IErrorListener listener)
        {
            List<T> objs = new List<T>();
            int offset = node.End - 1;
            //if (node.InnerContent.StartsWith(nodeType))
            //    offset += node.InnerContent.IndexOf(nodeType) + nodeType.Length + 1;
            content = PurgeEscapeChars(content);

            string currentType = "";
            foreach (var param in content.Split(' ').Reverse())
            {
                if (param != "" && param != nodeType)
                {
                    var typedParam = param;
                    if (typedParam.Contains(ASTTokens.TypeToken))
                    {
                        currentType = typedParam.Substring(typedParam.IndexOf(ASTTokens.TypeToken) + ASTTokens.TypeToken.Length);
                        if (typedParam.Substring(0, typedParam.IndexOf(ASTTokens.TypeToken)).Trim() == "")
                            continue;
                    }
                    else if (!typedParam.Contains(ASTTokens.TypeToken) && currentType != "")
                        typedParam = $"{typedParam}{ASTTokens.TypeToken}{currentType}";

                    var parsed = new ExpVisitor().Visit(new ASTNode(
                        offset - param.Length,
                        offset,
                        node.Line,
                        typedParam,
                        typedParam), parent, listener);
                    if (parsed is T nExp)
                        objs.Add(nExp);
                    else
                    {
                        listener.AddError(new ParseError(
                            $"Unexpected node type while parsing! Expected '{nodeType}' but got {nameof(T)}!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing,
                            parsed.Line,
                            parsed.Start));
                    }
                }
                offset -= param.Length + 1;
            }
            objs.Reverse();
            return objs;
        }

        internal static List<T> ParseAsList<T>(ASTNode node, INode parent, IErrorListener listener, bool throwIfNotCorrect = true)
        {
            List<T> items = new List<T>();
            foreach (var child in node.Children)
            {
                var newNode = new ExpVisitor().Visit(child, parent, listener);
                if (newNode is T nExp)
                    items.Add(nExp);
                else if (throwIfNotCorrect)
                    listener.AddError(new ParseError(
                        $"Could not parse predicate!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        child.Line,
                        child.Start));
            }
            return items;
        }

        internal static bool DoesContentContainTarget(ASTNode node, string nodeName, string targetName, IErrorListener listener)
        {
            if (!node.InnerContent.Contains(targetName))
            {
                listener.AddError(new ParseError(
                    $"'{nodeName}' is malformed! missing '{targetName}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static bool DoesContentContainNLooseChildren(ASTNode node, string nodeName, int target, IErrorListener listener)
        {
            var looseChildren = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, nodeName));
            var split = looseChildren.Split(' ');
            var actualCount = split.Length;
            if (split.Length == 1)
                if (split[0] == "")
                    actualCount--;
            if (actualCount != target)
            {
                listener.AddError(new ParseError(
                    $"'{nodeName}' is malformed! Expected {target} loose children but got {actualCount}.",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal static bool IsOfValidNodeType(string content, string nodeType)
        {
            if (content.StartsWith(nodeType))
            {
                if (nodeType.Length == content.Length)
                    return true;
                var nextCharacter = content[nodeType.Length];
                if (nextCharacter == ' ')
                    return true;
                if (nextCharacter == '(')
                    return true;
                if (nextCharacter == '\n')
                    return true;
            }
            return false;
        }

        internal static string RemoveNodeTypeAndEscapeChars(string content, string nodeType)
        {
            return PurgeEscapeChars(content).Remove(content.IndexOf(nodeType), nodeType.Length).Trim();
        }

        internal static string ReduceToSingleSpace(string text)
        {
            while (text.Contains("  "))
                text = text.Replace("  ", " ");
            return text;
        }

        internal static string PurgeEscapeChars(string str) => str.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
    }
}
