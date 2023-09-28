using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Parsers.Visitors
{
    public partial class ParserVisitor
    {
        public IErrorListener Listener { get; set; }

        public ParserVisitor(IErrorListener listener)
        {
            Listener = listener;
        }

        public T TryVisitAs<T>(ASTNode node, INode? parent) where T : INode
        {
            var res = VisitAs<T>(node, parent);
            if (res is T model)
                return model;
            Listener.AddError(new ParseError(
                $"Could not parse node as a '{nameof(T)}', got a '{nameof(res)}'",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing,
                node.Line,
                node.Start));
            return default(T);
        }

        private INode? VisitAs<T>(ASTNode node, INode? parent) where T : INode =>
            // Domain
            typeof(T) == typeof(DomainDecl)         ? TryVisitDomainDeclNode(node, parent) :
            typeof(T) == typeof(DomainNameDecl)     ? TryVisitDomainNameNode(node, parent) :
            typeof(T) == typeof(RequirementsDecl)   ? TryVisitRequirementsNode(node, parent) :
            typeof(T) == typeof(ExtendsDecl)        ? TryVisitExtendsNode(node, parent) :
            typeof(T) == typeof(TypesDecl)          ? TryVisitTypesNode(node, parent) :
            typeof(T) == typeof(ConstantsDecl)      ? TryVisitConstantsNode(node, parent) :
            typeof(T) == typeof(PredicatesDecl)     ? TryVisitPredicatesNode(node, parent) :
            typeof(T) == typeof(FunctionsDecl)      ? TryVisitFunctionsNode(node, parent) :
            typeof(T) == typeof(TimelessDecl)       ? TryVisitTimelessNode(node, parent) :
            typeof(T) == typeof(ActionDecl)         ? TryVisitActionNode(node, parent) :
            typeof(T) == typeof(DurativeActionDecl) ? TryVisitDurativeActionNode(node, parent) :
            typeof(T) == typeof(AxiomDecl)          ? TryVisitAxiomNode(node, parent) :

            // Problem
            typeof(T) == typeof(ProblemDecl)        ? TryVisitProblemDeclNode(node, parent) :
            typeof(T) == typeof(ProblemNameDecl)    ? TryVisitProblemNameNode(node, parent) :
            typeof(T) == typeof(DomainNameRefDecl)  ? TryVisitDomainRefNameNode(node, parent) :
            typeof(T) == typeof(ObjectsDecl)        ? TryVisitObjectsNode(node, parent) :
            typeof(T) == typeof(InitDecl)           ? TryVisitInitsNode(node, parent) :
            typeof(T) == typeof(GoalDecl)           ? TryVisitGoalNode(node, parent) :
            typeof(T) == typeof(MetricDecl)         ? TryVisitMetricNode(node, parent) :

            // Exp
            typeof(T) == typeof(WhenExp)            ? TryVisitWhenNode(node, parent) :
            typeof(T) == typeof(AndExp)             ? TryVisitAndNode(node, parent) :
            typeof(T) == typeof(OrExp)              ? TryVisitOrNode(node, parent) :
            typeof(T) == typeof(NotExp)             ? TryVisitNotNode(node, parent) :
            typeof(T) == typeof(PredicateExp)       ? TryVisitPredicateNode(node, parent) :
            typeof(T) == typeof(NumericExp)         ? TryVisitNumericNode(node, parent) :
            typeof(T) == typeof(NameExp)            ? TryVisitNameNode(node, parent) :
            typeof(T) == typeof(IExp)               ? VisitExp(node, parent) :

            // Default
            null;

        internal string ReplaceRangeWithSpaces(string text, int from, int to)
        {
            var newText = text.Substring(0, from);
            newText += new string(' ', to - from);
            newText += text.Substring(to);
            return newText;
        }

        internal bool DoesNotContainStrayCharacters(ASTNode node, string targetName)
        {
            if (node.InnerContent.Replace(targetName, "").Trim() != "")
            {
                Listener.AddError(new ParseError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.OuterContent.Replace(targetName, "").Trim()}",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal bool DoesNodeHaveSpecificChildCount(ASTNode node, string nodeName, int targetChildren)
        {
            if (targetChildren == 0)
            {
                if (node.Children.Count != 0)
                {
                    Listener.AddError(new ParseError(
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
                    Listener.AddError(new ParseError(
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

        internal bool DoesNodeHaveMoreThanNChildren(ASTNode node, string nodeName, int targetChildren)
        {
            if (node.Children.Count <= targetChildren)
            {
                Listener.AddError(new ParseError(
                    $"'{nodeName}' must have more than {targetChildren} children, but it has '{node.Children.Count}'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal List<T> LooseParseString<T>(ASTNode node, INode parent, string nodeType, string content)
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

                    var parsed = VisitExp(new ASTNode(
                        offset - param.Length,
                        offset,
                        node.Line,
                        typedParam,
                        typedParam), parent);
                    if (parsed is T nExp)
                        objs.Add(nExp);
                    else
                    {
                        Listener.AddError(new ParseError(
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

        internal List<T> ParseAsList<T>(ASTNode node, INode parent, bool throwIfNotCorrect = true)
        {
            List<T> items = new List<T>();
            foreach (var child in node.Children)
            {
                var newNode = VisitExp(child, parent);
                if (newNode is T nExp)
                    items.Add(nExp);
                else if (throwIfNotCorrect)
                    Listener.AddError(new ParseError(
                        $"Could not parse predicate!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        child.Line,
                        child.Start));
            }
            return items;
        }

        internal bool DoesContentContainTarget(ASTNode node, string nodeName, string targetName)
        {
            if (!node.InnerContent.Contains(targetName))
            {
                Listener.AddError(new ParseError(
                    $"'{nodeName}' is malformed! missing '{targetName}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal bool DoesContentContainNLooseChildren(ASTNode node, string nodeName, int target)
        {
            var looseChildren = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, nodeName));
            var split = looseChildren.Split(' ');
            var actualCount = split.Length;
            if (split.Length == 1)
                if (split[0] == "")
                    actualCount--;
            if (actualCount != target)
            {
                Listener.AddError(new ParseError(
                    $"'{nodeName}' is malformed! Expected {target} loose children but got {actualCount}.",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line,
                    node.Start));
                return false;
            }
            return true;
        }

        internal bool IsOfValidNodeType(string content, string nodeType)
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

        internal string RemoveNodeTypeAndEscapeChars(string content, string nodeType)
        {
            return PurgeEscapeChars(content).Remove(content.IndexOf(nodeType), nodeType.Length).Trim();
        }

        internal string ReduceToSingleSpace(string text)
        {
            while (text.Contains("  "))
                text = text.Replace("  ", " ");
            return text;
        }

        internal string PurgeEscapeChars(string str) => str.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
    }
}
