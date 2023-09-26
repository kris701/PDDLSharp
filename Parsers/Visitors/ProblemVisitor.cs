using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Parsers.Visitors
{
    public class ProblemVisitor : BaseVisitor, IVisitor<ASTNode, INode, IDecl>
    {
        public IDecl Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            IDecl? returnNode;
            if ((returnNode = TryVisitProblemDeclNode(node, parent, listener)) != null) return returnNode;
            if ((returnNode = TryVisitProblemNameNode(node, parent, listener)) != null) return returnNode;
            if ((returnNode = TryVisitDomainRefNameNode(node, parent, listener)) != null) return returnNode;
            if ((returnNode = TryVisitObjectsNode(node, parent, listener)) != null) return returnNode;
            if ((returnNode = TryVisitInitsNode(node, parent, listener)) != null) return returnNode;
            if ((returnNode = TryVisitGoalNode(node, parent, listener)) != null) return returnNode;
            if ((returnNode = TryVisitMetricNode(node, parent, listener)) != null) return returnNode;

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        public IDecl? TryVisitProblemDeclNode(ASTNode node, INode parent, IErrorListener listener)
        {
            if (IsOfValidNodeType(node.InnerContent, "define"))
            {
                if (DoesNotContainStrayCharacters(node, "define", listener))
                {
                    var returnProblem = new ProblemDecl(node);
                    foreach (var child in node.Children)
                    {
                        var visited = Visit(child, returnProblem, listener);

                        switch (visited)
                        {
                            case ProblemNameDecl d: returnProblem.Name = d; break;
                            case DomainNameRefDecl d: returnProblem.DomainName = d; break;
                            case ObjectsDecl d: returnProblem.Objects = d; break;
                            case InitDecl d: returnProblem.Init = d; break;
                            case GoalDecl d: returnProblem.Goal = d; break;
                            case MetricDecl d: returnProblem.Metric = d; break;
                        }
                    }
                    return returnProblem;
                }
            }
            return null;
        }

        public IDecl? TryVisitProblemNameNode(ASTNode node, INode parent, IErrorListener listener)
        {
            if (IsOfValidNodeType(node.InnerContent, "problem") &&
                DoesContentContainNLooseChildren(node, "problem", 1, listener))
            {
                var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "problem");
                return new ProblemNameDecl(node, parent, name);
            }
            return null;
        }

        public IDecl? TryVisitDomainRefNameNode(ASTNode node, INode parent, IErrorListener listener)
        {
            if (IsOfValidNodeType(node.InnerContent, ":domain") &&
                DoesContentContainNLooseChildren(node, ":domain", 1, listener))
            {
                var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":domain");
                return new DomainNameRefDecl(node, parent, name);
            }
            return null;
        }

        public IDecl? TryVisitObjectsNode(ASTNode node, INode parent, IErrorListener listener)
        {
            if (IsOfValidNodeType(node.InnerContent, ":objects") &&
                DoesNodeHaveSpecificChildCount(node, ":objects", 0, listener))
            {
                var newObjs = new ObjectsDecl(node, parent, new List<NameExp>());

                var parseStr = node.InnerContent.Substring(node.InnerContent.IndexOf(":objects") + ":objects".Length);
                newObjs.Objs = LooseParseString<NameExp>(node, newObjs, ":objects", parseStr, listener);

                return newObjs;
            }
            return null;
        }

        public IDecl? TryVisitInitsNode(ASTNode node, INode parent, IErrorListener listener)
        {
            if (IsOfValidNodeType(node.InnerContent, ":init") &&
                DoesNotContainStrayCharacters(node, ":init", listener))
            {
                var newInit = new InitDecl(node, parent, new List<IExp>());
                var preds = ParseAsList<PredicateExp>(node, newInit, listener, false);
                var nums = ParseAsList<NumericExp>(node, newInit, listener, false);
                newInit.Predicates.AddRange(preds);
                newInit.Predicates.AddRange(nums);
                return newInit;
            }
            return null;
        }

        public IDecl? TryVisitGoalNode(ASTNode node, INode parent, IErrorListener listener)
        {
            if (IsOfValidNodeType(node.InnerContent, ":goal") &&
                DoesNodeHaveSpecificChildCount(node, ":goal", 1, listener) &&
                DoesNotContainStrayCharacters(node, ":goal", listener))
            {
                var newGoal = new GoalDecl(node, parent, null);
                newGoal.GoalExp = new ExpVisitor().Visit(node.Children[0], newGoal, listener);
                return newGoal;
            }
            return null;
        }

        private static HashSet<string> MetricNodeTypes = new HashSet<string>()
        {
            "maximize", "minimize"
        };
        public IDecl? TryVisitMetricNode(ASTNode node, INode parent, IErrorListener listener)
        {
            if (IsOfValidNodeType(node.InnerContent, ":metric") &&
                DoesNodeHaveSpecificChildCount(node, ":metric", 1, listener) &&
                DoesContentContainNLooseChildren(node, ":metric", 1, listener))
            {
                var metricType = node.InnerContent.Substring(node.InnerContent.IndexOf(":metric") + ":metric".Length).Trim();
                if (MetricNodeTypes.Contains(metricType))
                {
                    var newMetric = new MetricDecl(node, parent, metricType, null);
                    newMetric.MetricExp = new ExpVisitor().Visit(node.Children[0], newMetric, listener);
                    return newMetric;
                }
            }
            return null;
        }
    }
}
