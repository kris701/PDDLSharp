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
    public class ExpVisitor : BaseVisitor, IVisitor<ASTNode, INode, IExp>
    {
        public IExp Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            IExp returnNode = null;
            if (TryVisitAndNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitOrNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitNotNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitNumericNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitPredicateNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitNameNode(node, parent, listener, out returnNode))
                return returnNode;

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return default;
        }

        public bool TryVisitAndNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (IsOfValidNodeType(node.InnerContent, "and"))
            {
                if (DoesNodeHaveMoreThanNChildren(node, "and", 0, listener) &&
                    DoesNotContainStrayCharacters(node, "and", listener))
                {
                    var newAndExp = new AndExp(node, parent, new List<IExp>());
                    foreach (var child in node.Children)
                        newAndExp.Children.Add(Visit(child, newAndExp, listener));

                    exp = newAndExp;
                    return true;
                }
            }
            exp = null;
            return false;
        }

        public bool TryVisitOrNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (IsOfValidNodeType(node.InnerContent, "or"))
            {
                if (DoesNodeHaveSpecificChildCount(node, "or", 2, listener) &&
                    DoesNotContainStrayCharacters(node, "or", listener))
                {

                    var newOrExp = new OrExp(node, parent, null, null);
                    newOrExp.Option1 = Visit(node.Children[0], newOrExp, listener);
                    newOrExp.Option2 = Visit(node.Children[1], newOrExp, listener);
                    exp = newOrExp;
                    return true;
                }
            }
            exp = null;
            return false;
        }

        public bool TryVisitNotNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (IsOfValidNodeType(node.InnerContent, "not"))
            {
                if (DoesNodeHaveSpecificChildCount(node, "not", 1, listener) &&
                    DoesNotContainStrayCharacters(node, "not", listener))
                {
                    var newNotExp = new NotExp(node, parent, null);
                    newNotExp.Child = Visit(node.Children[0], newNotExp, listener);
                    exp = newNotExp;
                    return true;
                }
            }
            exp = null;
            return false;
        }

        public bool TryVisitPredicateNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (node.OuterContent.Contains('(') && node.OuterContent.Contains(')') && node.InnerContent != "")
            {
                if (DoesNodeHaveSpecificChildCount(node, "predicate", 0, listener))
                {
                    var predicateName = node.InnerContent.Split(' ')[0];
                    var newPredicateExp = new PredicateExp(node, parent, predicateName, new List<NameExp>());

                    var content = node.InnerContent.Substring(node.InnerContent.IndexOf(predicateName) + predicateName.Length);
                    newPredicateExp.Arguments = LooseParseString<NameExp>(node, newPredicateExp, predicateName, content, listener);

                    exp = newPredicateExp;
                    return true;
                }
            } 
            exp = null;
            return false;
        }

        private static HashSet<string> NumericNodeTypes = new HashSet<string>()
        {
            "increase", "decrease", "assign", "scale-up", "scale-down", "=", "+", "-", "*", "/", "<", ">"
        };
        public bool TryVisitNumericNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            exp = null;
            if (node.OuterContent.Contains('(') && node.OuterContent.Contains(')') && node.InnerContent != "")
            {
                if (node.Children.Count >= 1)
                {
                    var numericName = node.InnerContent.Split(' ')[0].Trim();
                    if (NumericNodeTypes.Contains(numericName))
                    {
                        var newNumericExp = new NumericExp(node, parent, numericName, null, null);
                        IExp arg1;
                        IExp arg2;
                        if (node.Children.Count == 2)
                        {
                            arg1 = Visit(node.Children[0], newNumericExp, listener);
                            if (arg1 == null)
                                return false;
                            arg2 = Visit(node.Children[1], newNumericExp, listener);
                            if (arg2 == null)
                                return false;
                        }
                        else
                        {
                            arg1 = Visit(node.Children[0], newNumericExp, listener);
                            if (arg1 == null)
                                return false;
                            var content = node.InnerContent.Substring(node.InnerContent.IndexOf(numericName) + numericName.Length);
                            arg2 = Visit(new ASTNode(node.Start, node.End, content, content), newNumericExp, listener);
                            if (arg2 == null)
                                return false;
                        }
                        newNumericExp.Arg1 = arg1;
                        newNumericExp.Arg2 = arg2;
                        exp = newNumericExp;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryVisitNameNode(ASTNode node, INode parent, IErrorListener listener, out IExp exp)
        {
            if (node.InnerContent.Contains(ASTTokens.TypeToken))
            {
                if (DoesNodeHaveSpecificChildCount(node, "name", 0, listener))
                {
                    var left = node.InnerContent.Substring(0, node.InnerContent.IndexOf(ASTTokens.TypeToken)).Trim();
                    var right = node.InnerContent.Substring(node.InnerContent.IndexOf(ASTTokens.TypeToken) + 3).Trim();

                    if (left == "")
                    {
                        listener.AddError(new ParseError(
                            $"Context indicated the use of a type, but an object name was not given!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing,
                            node.Line,
                            node.Start));
                    }
                    if (right == "")
                    {
                        listener.AddError(new ParseError(
                            $"Context indicated the use of a type, but a type was not given!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Parsing,
                            node.Line,
                            node.Start));
                    }

                    var newNameExp = new NameExp(node, parent, left);
                    //var newNameExp = new NameExp(node, parent, left.Replace("?", ""));
                    newNameExp.Type = new TypeExp(
                        new ASTNode(
                            node.Start + left.Length + 3,
                            node.Start + left.Length + 3 + right.Length,
                            right,
                            right),
                        newNameExp,
                        right);
                    exp = newNameExp;
                    return true;
                }
            }
            else
            {
                if (DoesNodeHaveSpecificChildCount(node, "name", 0, listener))
                {
                    var newNameExp = new NameExp(node, parent, node.InnerContent);
                    //var newNameExp = new NameExp(node, parent, node.InnerContent.Replace("?", ""));
                    exp = newNameExp;
                    return true;
                }
            }
            exp = null;
            return false;
        }
    }
}
