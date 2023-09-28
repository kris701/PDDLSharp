using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Expressions;
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
        public IExp VisitExp(ASTNode node, INode? parent)
        {
            IExp? returnNode;
            if ((returnNode = TryVisitAndNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitWhenNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitOrNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitNotNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitNumericNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitPredicateNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitNameNode(node, parent)) != null) return returnNode;

            Listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        public IExp? TryVisitWhenNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "when") &&
                DoesNodeHaveSpecificChildCount(node, "when", 2) &&
                DoesNotContainStrayCharacters(node, "when"))
            {
                var newWhenExp = new WhenExp(node, parent, null, null);
                newWhenExp.Condition = VisitExp(node.Children[0], newWhenExp);
                newWhenExp.Effect = VisitExp(node.Children[1], newWhenExp);

                return newWhenExp;
            }
            return null;
        }

        public IExp? TryVisitAndNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "and") &&
                DoesNodeHaveMoreThanNChildren(node, "and", 0) &&
                DoesNotContainStrayCharacters(node, "and"))
            {
                var newAndExp = new AndExp(node, parent, new List<IExp>());
                foreach (var child in node.Children)
                    newAndExp.Children.Add(VisitExp(child, newAndExp));
                return newAndExp;
            }
            return null;
        }

        public IExp? TryVisitOrNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "or") &&
                DoesNodeHaveSpecificChildCount(node, "or", 2) &&
                DoesNotContainStrayCharacters(node, "or"))
            {
                var newOrExp = new OrExp(node, parent, null, null);
                newOrExp.Option1 = VisitExp(node.Children[0], newOrExp);
                newOrExp.Option2 = VisitExp(node.Children[1], newOrExp);
                return newOrExp;
            }
            return null;
        }

        public IExp? TryVisitNotNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "not") &&
                DoesNodeHaveSpecificChildCount(node, "not", 1) &&
                DoesNotContainStrayCharacters(node, "not"))
            {
                var newNotExp = new NotExp(node, parent, null);
                newNotExp.Child = VisitExp(node.Children[0], newNotExp);
                return newNotExp;
            }
            return null;
        }

        public IExp? TryVisitPredicateNode(ASTNode node, INode? parent)
        {
            if (node.OuterContent.Contains('(') && 
                node.OuterContent.Contains(')') && 
                node.InnerContent != "" &&
                DoesNodeHaveSpecificChildCount(node, "predicate", 0))
            {
                var predicateName = node.InnerContent.Split(' ')[0];
                var newPredicateExp = new PredicateExp(node, parent, predicateName, new List<NameExp>());

                var content = node.InnerContent.Substring(node.InnerContent.IndexOf(predicateName) + predicateName.Length);
                newPredicateExp.Arguments = LooseParseString<NameExp>(node, newPredicateExp, predicateName, content);

                return newPredicateExp;
            }
            return null;
        }

        private static HashSet<string> NumericNodeTypes = new HashSet<string>()
        {
            "increase", "decrease", "assign", "scale-up", "scale-down", "=", "+", "-", "*", "/", "<", ">"
        };

        public IExp? TryVisitNumericNode(ASTNode node, INode? parent)
        {
            if (node.OuterContent.Contains('(') && 
                node.OuterContent.Contains(')') && 
                node.InnerContent != "" &&
                node.Children.Count >= 1)
            {
                var numericName = node.InnerContent.Split(' ')[0].Trim();
                if (NumericNodeTypes.Contains(numericName))
                {
                    var newNumericExp = new NumericExp(node, parent, numericName, null, null);
                    IExp arg1;
                    IExp arg2;
                    if (node.Children.Count == 2)
                    {
                        arg1 = VisitExp(node.Children[0], newNumericExp);
                        if (arg1 == null)
                            return null;
                        arg2 = VisitExp(node.Children[1], newNumericExp);
                        if (arg2 == null)
                            return null;
                    }
                    else
                    {
                        arg1 = VisitExp(node.Children[0], newNumericExp);
                        if (arg1 == null)
                            return null;
                        var content = node.InnerContent.Substring(node.InnerContent.IndexOf(numericName) + numericName.Length);
                        arg2 = VisitExp(new ASTNode(node.Start, node.End, content, content), newNumericExp);
                        if (arg2 == null)
                            return null;
                    }
                    newNumericExp.Arg1 = arg1;
                    newNumericExp.Arg2 = arg2;
                    return newNumericExp;
                }
            }
            return null;
        }

        public IExp? TryVisitNameNode(ASTNode node, INode? parent)
        {
            if (node.InnerContent.Contains(ASTTokens.TypeToken) &&
                DoesNodeHaveSpecificChildCount(node, "name", 0))
            {
                var left = node.InnerContent.Substring(0, node.InnerContent.IndexOf(ASTTokens.TypeToken)).Trim();
                var right = node.InnerContent.Substring(node.InnerContent.IndexOf(ASTTokens.TypeToken) + 3).Trim();

                if (left == "")
                {
                    Listener.AddError(new ParseError(
                        $"Context indicated the use of a type, but an object name was not given!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
                }
                if (right == "")
                {
                    Listener.AddError(new ParseError(
                        $"Context indicated the use of a type, but a type was not given!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line,
                        node.Start));
                }

                var newNameExp = new NameExp(node, parent, left);
                newNameExp.Type = new TypeExp(
                    new ASTNode(
                        node.Start + left.Length + 3,
                        node.Start + left.Length + 3 + right.Length,
                        right,
                        right),
                    newNameExp,
                    right);
                return newNameExp;
            }
            else if (DoesNodeHaveSpecificChildCount(node, "name", 0))
            {
                var newNameExp = new NameExp(node, parent, node.InnerContent);
                return newNameExp;
            }
            return null;
        }
    }
}
