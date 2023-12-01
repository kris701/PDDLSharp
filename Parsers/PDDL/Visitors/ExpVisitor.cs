using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Parsers.Visitors
{
    public partial class ParserVisitor
    {
        public IExp VisitExp(ASTNode node, INode? parent)
        {
            IExp? returnNode;
            if ((returnNode = TryVisitAndNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitWhenNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitTimedLiteralNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitForAllNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitExistsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitImplyNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitOrNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitNotNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitNumericNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitPredicateNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitLiteralNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitNameNode(node, parent)) != null) return returnNode;

            Listener.AddError(new PDDLSharpError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        public IExp? TryVisitTimedLiteralNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "at") &&
                node.Children.Count == 1 &&
                node.InnerContent.Any(char.IsDigit))
            {
                var newTimedLiteralExp = new TimedLiteralExp(node, parent, -1);

                var stray = node.InnerContent.Substring(node.InnerContent.IndexOf("at") + "at".Length).Trim();
                var value = Convert.ToInt32(stray);
                newTimedLiteralExp.Value = value;
                newTimedLiteralExp.Literal = VisitExp(node.Children[0], newTimedLiteralExp);

                return newTimedLiteralExp;
            }
            return null;
        }

        public IExp? TryVisitImplyNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "imply") &&
                DoesNodeHaveSpecificChildCount(node, "imply", 2) &&
                DoesNotContainStrayCharacters(node, "imply"))
            {
                var newImplyExp = new ImplyExp(node, parent);
                newImplyExp.Antecedent = VisitExp(node.Children[0], newImplyExp);
                newImplyExp.Consequent = VisitExp(node.Children[1], newImplyExp);

                return newImplyExp;
            }
            return null;
        }

        public IExp? TryVisitExistsNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "exists") &&
                DoesNodeHaveSpecificChildCount(node, "exists", 2) &&
                DoesNotContainStrayCharacters(node, "exists"))
            {
                var newExistsExp = new ExistsExp(node, parent);
                newExistsExp.Parameters = new ParameterExp(
                    node.Children[0],
                    newExistsExp,
                    ParseAsParameters(node.Children[0], newExistsExp, "", node.Children[0].InnerContent));
                newExistsExp.Expression = VisitExp(node.Children[1], newExistsExp);

                return newExistsExp;
            }
            return null;
        }

        public IExp? TryVisitForAllNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "forall") &&
                DoesNodeHaveSpecificChildCount(node, "forall", 2) &&
                DoesNotContainStrayCharacters(node, "forall"))
            {
                var newForAllExpression = new ForAllExp(node, parent);
                newForAllExpression.Parameters = new ParameterExp(
                    node.Children[0],
                    newForAllExpression,
                    ParseAsParameters(node.Children[0], newForAllExpression, "", node.Children[0].InnerContent));
                newForAllExpression.Expression = VisitExp(node.Children[1], newForAllExpression);

                return newForAllExpression;
            }
            return null;
        }

        public IExp? TryVisitWhenNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "when") &&
                DoesNodeHaveSpecificChildCount(node, "when", 2) &&
                DoesNotContainStrayCharacters(node, "when"))
            {
                var newWhenExp = new WhenExp(node, parent);
                newWhenExp.Condition = VisitExp(node.Children[0], newWhenExp);
                newWhenExp.Effect = VisitExp(node.Children[1], newWhenExp);

                return newWhenExp;
            }
            return null;
        }

        public IExp? TryVisitAndNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "and") &&
                //DoesNodeHaveMoreThanNChildren(node, "and", 0) &&
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
                DoesNotContainStrayCharacters(node, "or"))
            {
                var newOrExp = new OrExp(node, parent);
                foreach (var child in node.Children)
                    newOrExp.Options.Add(VisitExp(child, newOrExp));
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
                var newNotExp = new NotExp(node, parent);
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
                var newPredicateExp = new PredicateExp(node, parent, predicateName);

                var content = node.InnerContent.Substring(node.InnerContent.IndexOf(predicateName) + predicateName.Length);
                newPredicateExp.Arguments = ParseAsParameters(node, newPredicateExp, predicateName, content);

                return newPredicateExp;
            }
            return null;
        }

        private static readonly HashSet<string> NumericNodeTypes = new HashSet<string>()
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
                node.InnerContent = ReduceToSingleSpace(node.InnerContent);
                var numericName = node.InnerContent.Split(' ')[0].Trim();
                if (NumericNodeTypes.Contains(numericName))
                {
                    var newNumericExp = new NumericExp(node, parent, numericName);
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
                        var content = node.InnerContent.Substring(node.InnerContent.IndexOf(numericName) + numericName.Length).Trim();
                        arg2 = VisitExp(new ASTNode(node.Line, content, content), newNumericExp);
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

        public IExp? TryVisitLiteralNode(ASTNode node, INode? parent)
        {
            if (node.Children.Count == 0 &&
                node.InnerContent.Any(char.IsDigit) &&
                !node.InnerContent.Any(char.IsLetter))
            {
                var newLiteralExp = new LiteralExp(node, parent, -1);
                var value = Convert.ToInt32(node.InnerContent);
                newLiteralExp.Value = value;

                return newLiteralExp;
            }
            return null;
        }

        public IExp? TryVisitNameNode(ASTNode node, INode? parent)
        {
            if (node.InnerContent.Contains(PDDLASTTokens.TypeToken) &&
                DoesNodeHaveSpecificChildCount(node, "name", 0))
            {
                var left = node.InnerContent.Substring(0, node.InnerContent.IndexOf(PDDLASTTokens.TypeToken)).Trim();
                var right = node.InnerContent.Substring(node.InnerContent.IndexOf(PDDLASTTokens.TypeToken) + 3).Trim();

                if (left == "")
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Context indicated the use of a type, but an object name was not given!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line));
                }
                if (right == "")
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Context indicated the use of a type, but a type was not given!",
                        ParseErrorType.Error,
                        ParseErrorLevel.Parsing,
                        node.Line));
                }

                var newNameExp = new NameExp(node, parent, left);
                newNameExp.Type = new TypeExp(
                    new ASTNode(
                        node.Line,
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
