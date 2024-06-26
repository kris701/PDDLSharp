﻿using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using PDDLSharp.Models.PDDL.Shared;

namespace PDDLSharp.Parsers.Visitors
{
    public partial class ParserVisitor
    {
        public IDecl? VisitProblem(ASTNode node, INode? parent)
        {
            IDecl? returnNode;
            if ((returnNode = TryVisitProblemDeclNode(node)) != null) return returnNode;
            if ((returnNode = TryVisitProblemNameNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitRequirementsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitDomainRefNameNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitSituationNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitObjectsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitInitsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitGoalNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitMetricNode(node, parent)) != null) return returnNode;

            Listener.AddError(new PDDLSharpError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        public IDecl? TryVisitProblemDeclNode(ASTNode node)
        {
            if (IsOfValidNodeType(node.InnerContent, "define") &&
                DoesNotContainStrayCharacters(node, "define"))
            {
                var returnProblem = new ProblemDecl(node);
                foreach (var child in node.Children)
                {
                    var visited = VisitProblem(child, returnProblem);

                    switch (visited)
                    {
                        case ProblemNameDecl d: returnProblem.Name = d; break;
                        case DomainNameRefDecl d: returnProblem.DomainName = d; break;
                        case RequirementsDecl d: returnProblem.Requirements = d; break;
                        case ObjectsDecl d: returnProblem.Objects = d; break;
                        case InitDecl d: returnProblem.Init = d; break;
                        case GoalDecl d: returnProblem.Goal = d; break;
                        case MetricDecl d: returnProblem.Metric = d; break;
                    }
                }
                return returnProblem;
            }
            return null;
        }

        public IDecl? TryVisitProblemNameNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "problem") &&
                DoesContentContainNLooseChildren(node, "problem", 1))
            {
                var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "problem");
                return new ProblemNameDecl(node, parent, name);
            }
            return null;
        }

        public IDecl? TryVisitDomainRefNameNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":domain") &&
                DoesContentContainNLooseChildren(node, ":domain", 1))
            {
                var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":domain");
                return new DomainNameRefDecl(node, parent, name);
            }
            return null;
        }

        public IDecl? TryVisitSituationNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":situation") &&
                DoesContentContainNLooseChildren(node, ":situation", 1))
            {
                var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":situation");
                return new SituationDecl(node, parent, name);
            }
            return null;
        }

        public IDecl? TryVisitObjectsNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":objects") &&
                DoesNodeHaveSpecificChildCount(node, ":objects", 0))
            {
                var newObjs = new ObjectsDecl(node, parent, new List<NameExp>());

                var parseStr = node.InnerContent.Substring(node.InnerContent.IndexOf(":objects") + ":objects".Length);
                newObjs.Objs = ParseAsParameters(node, newObjs, ":objects", parseStr);

                return newObjs;
            }
            return null;
        }

        public IDecl? TryVisitInitsNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":init") &&
                DoesNotContainStrayCharacters(node, ":init"))
            {
                var newInit = new InitDecl(node, parent, new List<IExp>());
                var preds = ParseAsList<IExp>(node, newInit, false);
                newInit.Predicates.AddRange(preds);
                return newInit;
            }
            return null;
        }

        public IDecl? TryVisitGoalNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":goal") &&
                DoesNodeHaveSpecificChildCount(node, ":goal", 1) &&
                DoesNotContainStrayCharacters(node, ":goal"))
            {
                var newGoal = new GoalDecl(node, parent);
                newGoal.GoalExp = VisitExp(node.Children[0], newGoal);
                return newGoal;
            }
            return null;
        }

        private static readonly HashSet<string> MetricNodeTypes = new HashSet<string>()
        {
            "maximize", "minimize"
        };
        private static string MetricNodeTypesStr()
        {
            string retStr = "";
            foreach (var metric in MetricNodeTypes)
                retStr += $"{metric}, ";
            return retStr;
        }

        public IDecl? TryVisitMetricNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":metric") &&
                DoesNodeHaveSpecificChildCount(node, ":metric", 1) &&
                DoesContentContainNLooseChildren(node, ":metric", 1))
            {
                var metricType = node.InnerContent.Substring(node.InnerContent.IndexOf(":metric") + ":metric".Length).Trim();
                if (MetricNodeTypes.Contains(metricType))
                {
                    var newMetric = new MetricDecl(node, parent, metricType);
                    newMetric.MetricExp = VisitExp(node.Children[0], newMetric);
                    return newMetric;
                }
                else
                    Listener.AddError(new PDDLSharpError(
                        $"Invalid metric node type '{metricType}'. Allowed types are: {MetricNodeTypesStr()}",
                        ParseErrorType.Error,
                        ParseErrorLevel.Analyser,
                        node.Line));
            }
            return null;
        }
    }
}
