﻿using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL.Shared;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("PDDLSharp.Parsers.Tests")]
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
            if (res is null)
                Listener.AddError(new PDDLSharpError(
                    $"Could not parse node as a '{typeof(T)}', got a 'null'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line));
            else
                Listener.AddError(new PDDLSharpError(
                    $"Could not parse node as a '{typeof(T)}', got a '{res.GetType().Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line));
            return default(T);
        }

        private INode? VisitAs<T>(ASTNode node, INode? parent) where T : INode =>
            // Domain
            typeof(T) == typeof(DomainDecl) ? TryVisitDomainDeclNode(node, parent) :
            typeof(T) == typeof(DomainNameDecl) ? TryVisitDomainNameNode(node, parent) :
            typeof(T) == typeof(RequirementsDecl) ? TryVisitRequirementsNode(node, parent) :
            typeof(T) == typeof(ExtendsDecl) ? TryVisitExtendsNode(node, parent) :
            typeof(T) == typeof(TypesDecl) ? TryVisitTypesNode(node, parent) :
            typeof(T) == typeof(ConstantsDecl) ? TryVisitConstantsNode(node, parent) :
            typeof(T) == typeof(PredicatesDecl) ? TryVisitPredicatesNode(node, parent) :
            typeof(T) == typeof(FunctionsDecl) ? TryVisitFunctionsNode(node, parent) :
            typeof(T) == typeof(TimelessDecl) ? TryVisitTimelessNode(node, parent) :
            typeof(T) == typeof(ActionDecl) ? TryVisitActionNode(node, parent) :
            typeof(T) == typeof(DurativeActionDecl) ? TryVisitDurativeActionNode(node, parent) :
            typeof(T) == typeof(AxiomDecl) ? TryVisitAxiomNode(node, parent) :
            typeof(T) == typeof(DerivedDecl) ? TryVisitDerivedNode(node, parent) :

            // Problem
            typeof(T) == typeof(ProblemDecl) ? TryVisitProblemDeclNode(node) :
            typeof(T) == typeof(ProblemNameDecl) ? TryVisitProblemNameNode(node, parent) :
            typeof(T) == typeof(DomainNameRefDecl) ? TryVisitDomainRefNameNode(node, parent) :
            typeof(T) == typeof(SituationDecl) ? TryVisitSituationNode(node, parent) :
            typeof(T) == typeof(ObjectsDecl) ? TryVisitObjectsNode(node, parent) :
            typeof(T) == typeof(InitDecl) ? TryVisitInitsNode(node, parent) :
            typeof(T) == typeof(GoalDecl) ? TryVisitGoalNode(node, parent) :
            typeof(T) == typeof(MetricDecl) ? TryVisitMetricNode(node, parent) :

            // Exp
            typeof(T) == typeof(WhenExp) ? TryVisitWhenNode(node, parent) :
            typeof(T) == typeof(ForAllExp) ? TryVisitForAllNode(node, parent) :
            typeof(T) == typeof(TimedLiteralExp) ? TryVisitTimedLiteralNode(node, parent) :
            typeof(T) == typeof(LiteralExp) ? TryVisitLiteralNode(node, parent) :
            typeof(T) == typeof(ExistsExp) ? TryVisitExistsNode(node, parent) :
            typeof(T) == typeof(ImplyExp) ? TryVisitImplyNode(node, parent) :
            typeof(T) == typeof(AndExp) ? TryVisitAndNode(node, parent) :
            typeof(T) == typeof(OrExp) ? TryVisitOrNode(node, parent) :
            typeof(T) == typeof(NotExp) ? TryVisitNotNode(node, parent) :
            typeof(T) == typeof(PredicateExp) ? TryVisitPredicateNode(node, parent) :
            typeof(T) == typeof(NumericExp) ? TryVisitNumericNode(node, parent) :
            typeof(T) == typeof(NameExp) ? TryVisitNameNode(node, parent) :
            typeof(T) == typeof(IExp) ? VisitExp(node, parent) :

            // Default
            TryCatchParsing<T>(node, parent);

        private INode TryCatchParsing<T>(ASTNode node, INode? parent)
        {
            if (typeof(T) == typeof(INode))
            {
                var tryArea = VisitDomain(node, parent);
                if (tryArea == null)
                    tryArea = VisitProblem(node, parent);
                return tryArea;
            }
            return null;
        }

        internal bool DoesNotContainStrayCharacters(ASTNode node, string targetName)
        {
            if (node.InnerContent.Replace(targetName, "").Trim() != "")
            {
                Listener.AddError(new PDDLSharpError(
                    $"The node '{targetName}' has unknown content inside! Contains stray characters: {node.OuterContent.Replace(targetName, "").Trim()}",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line));
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
                    Listener.AddError(new PDDLSharpError(
                        $"'{nodeName}' must not contain any children!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line));
                    return false;
                }
            }
            else
            {
                if (node.Children.Count != targetChildren)
                {
                    Listener.AddError(new PDDLSharpError(
                        $"'{nodeName}' must have exactly {targetChildren} children, but it has '{node.Children.Count}'!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line));
                    return false;
                }
            }
            return true;
        }

        internal bool DoesNodeHaveMoreThanNChildren(ASTNode node, string nodeName, int targetChildren)
        {
            if (node.Children.Count <= targetChildren)
            {
                Listener.AddError(new PDDLSharpError(
                    $"'{nodeName}' must have more than {targetChildren} children, but it has '{node.Children.Count}'!",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line));
                return false;
            }
            return true;
        }

        internal List<NameExp> ParseAsParameters(ASTNode node, INode parent, string nodeType, string content)
        {
            List<NameExp> objs = new List<NameExp>();
            content = PurgeEscapeChars(content);

            string currentType = "object";
            foreach (var param in content.Split(' ').Reverse())
            {
                if (param != "" && param != nodeType)
                {
                    var typedParam = param;
                    if (typedParam.Contains(PDDLASTTokens.TypeToken))
                    {
                        currentType = typedParam.Substring(typedParam.IndexOf(PDDLASTTokens.TypeToken) + PDDLASTTokens.TypeToken.Length);
                        if (typedParam.Substring(0, typedParam.IndexOf(PDDLASTTokens.TypeToken)).Trim() == "")
                            continue;
                    }
                    else if (!typedParam.Contains(PDDLASTTokens.TypeToken) && currentType != "")
                        typedParam = $"{typedParam}{PDDLASTTokens.TypeToken}{currentType}";

                    var parsed = VisitAs<NameExp>(new ASTNode(
                        node.Line,
                        typedParam,
                        typedParam), parent);
                    if (parsed is NameExp nExp)
                        objs.Add(nExp);
                    else
                    {
                        if (parsed == null)
                        {
                            Listener.AddError(new PDDLSharpError(
                                $"Unexpected node type while parsing! Expected '{nameof(NameExp)}' but got null!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Parsing,
                                node.Line));
                        }
                        else
                        {
                            Listener.AddError(new PDDLSharpError(
                                $"Unexpected node type while parsing! Expected '{nameof(NameExp)}' but got '{parsed.GetType().Name}'!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Parsing,
                                parsed.Line));
                        }
                    }
                }
            }
            objs.Reverse();
            return objs;
        }

        internal List<T> ParseAsList<T>(ASTNode node, INode parent, bool throwIfNotCorrect = true) where T : INode
        {
            List<T> items = new List<T>();
            foreach (var child in node.Children)
            {
                var newNode = VisitAs<T>(child, parent);
                if (newNode is T nExp)
                    items.Add(nExp);
                else if (throwIfNotCorrect)
                    Listener.AddError(new PDDLSharpError(
                        $"Could not parse predicate!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        child.Line));
            }
            return items;
        }

        internal bool DoesContentContainTarget(ASTNode node, string nodeName, string targetName)
        {
            if (!node.InnerContent.Contains(targetName))
            {
                Listener.AddError(new PDDLSharpError(
                    $"'{nodeName}' is malformed! missing '{targetName}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line));
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
                Listener.AddError(new PDDLSharpError(
                    $"'{nodeName}' is malformed! Expected {target} loose children but got {actualCount}.",
                    ParseErrorType.Error,
                    ParseErrorLevel.Parsing,
                    node.Line));
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

        internal string RemoveNodeType(string content, string nodeType)
        {
            return content.Remove(content.IndexOf(nodeType), nodeType.Length).Trim();
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
