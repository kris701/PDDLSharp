using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.AST;
using PDDL.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Parsers.Visitors
{
    public class ProblemVisitor : BaseVisitor, IVisitor<ASTNode, INode, IDecl>
    {
        public IDecl Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            IDecl returnNode = null;
            if (TryVisitProblemDeclNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitProblemNameNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitDomainRefNameNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitObjectsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitInitsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitGoalNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitMetricNode(node, parent, listener, out returnNode))
                return returnNode;

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return default;
        }

        public bool TryVisitProblemDeclNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "define"))
            {
                if (DoesNotContainStrayCharacters(node, "define", listener))
                {
                    var returnProblem = new ProblemDecl(node);
                    foreach (var child in node.Children)
                    {
                        var visited = Visit(child, returnProblem, listener);
                        if (visited is ProblemNameDecl name)
                            returnProblem.Name = name;
                        else if (visited is DomainNameRefDecl domainName)
                            returnProblem.DomainName = domainName;
                        else if (visited is ObjectsDecl objects)
                            returnProblem.Objects = objects;
                        else if (visited is InitDecl inits)
                            returnProblem.Init = inits;
                        else if (visited is GoalDecl goal)
                            returnProblem.Goal = goal;
                        else if (visited is MetricDecl metric)
                            returnProblem.Metric = metric;
                    }
                    decl = returnProblem;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitProblemNameNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "problem"))
            {
                if (DoesContentContainNLooseChildren(node, "problem", 1, listener))
                {
                    var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "problem");
                    decl = new ProblemNameDecl(node, parent, name);
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitDomainRefNameNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":domain"))
            {
                if (DoesContentContainNLooseChildren(node, ":domain", 1, listener))
                {
                    var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":domain");
                    decl = new DomainNameRefDecl(node, parent, name);
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitObjectsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":objects"))
            {
                if (DoesNodeHaveSpecificChildCount(node, ":objects", 0, listener))
                {
                    var newObjs = new ObjectsDecl(node, parent, new List<NameExp>());

                    var parseStr = node.InnerContent.Substring(node.InnerContent.IndexOf(":objects") + ":objects".Length);
                    newObjs.Objs = LooseParseString<NameExp>(node, newObjs, ":objects", parseStr, listener);

                    decl = newObjs;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitInitsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":init"))
            {
                if (DoesNotContainStrayCharacters(node, ":init", listener))
                {
                    var newInit = new InitDecl(node, parent, new List<IExp>());
                    var preds = ParseAsList<PredicateExp>(node, newInit, listener, false);
                    var nums = ParseAsList<NumericExp>(node, newInit, listener, false);
                    newInit.Predicates.AddRange(preds);
                    newInit.Predicates.AddRange(nums);
                    decl = newInit;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitGoalNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":goal"))
            {
                if (DoesNodeHaveSpecificChildCount(node, ":goal", 1, listener) &&
                    DoesNotContainStrayCharacters(node, ":goal", listener))
                {
                    var newGoal = new GoalDecl(node, parent, null);
                    newGoal.GoalExp = new ExpVisitor().Visit(node.Children[0], newGoal, listener);
                    decl = newGoal;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        private static HashSet<string> MetricNodeTypes = new HashSet<string>()
        {
            "maximize", "minimize"
        };
        public bool TryVisitMetricNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":metric"))
            {
                if (DoesNodeHaveSpecificChildCount(node, ":metric", 1, listener) &&
                    DoesContentContainNLooseChildren(node, ":metric", 1, listener))
                {
                    var metricType = node.InnerContent.Substring(node.InnerContent.IndexOf(":metric") + ":metric".Length).Trim();
                    if (MetricNodeTypes.Contains(metricType))
                    {
                        var newMetric = new MetricDecl(node, parent, metricType, null);
                        newMetric.MetricExp = new ExpVisitor().Visit(node.Children[0], newMetric, listener);
                        decl = newMetric;
                        return true;
                    }
                }
            }
            decl = null;
            return false;
        }
    }
}
