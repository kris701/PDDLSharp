using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Parsers.Visitors
{
    public partial class ParserVisitor
    {
        public IDecl VisitDomain(ASTNode node, INode? parent)
        {
            IDecl? returnNode;
            if ((returnNode = TryVisitDomainDeclNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitDomainNameNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitRequirementsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitExtendsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitTypesNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitConstantsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitPredicatesNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitFunctionsNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitTimelessNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitActionNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitDurativeActionNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitAxiomNode(node, parent)) != null) return returnNode;
            if ((returnNode = TryVisitDerivedNode(node, parent)) != null) return returnNode;

            Listener.AddError(new PDDLSharpError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        public IDecl? TryVisitDomainDeclNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "define") &&
                DoesNotContainStrayCharacters(node, "define"))
            {
                var returnDomain = new DomainDecl(node);
                foreach (var child in node.Children)
                {
                    var visited = VisitDomain(child, returnDomain);

                    switch (visited)
                    {
                        case DomainNameDecl d: returnDomain.Name = d; break;
                        case RequirementsDecl d: returnDomain.Requirements = d; break;
                        case ExtendsDecl d: returnDomain.Extends = d; break;
                        case TypesDecl d: returnDomain.Types = d; break;
                        case ConstantsDecl d: returnDomain.Constants = d; break;
                        case TimelessDecl d: returnDomain.Timeless = d; break;
                        case PredicatesDecl d: returnDomain.Predicates = d; break;
                        case FunctionsDecl d: returnDomain.Functions = d; break;
                        case ActionDecl d: returnDomain.Actions.Add(d); break;
                        case DurativeActionDecl d: returnDomain.DurativeActions.Add(d); break;
                        case AxiomDecl d: returnDomain.Axioms.Add(d); break;
                        case DerivedDecl d: returnDomain.Deriveds.Add(d); break;
                    }
                }
                return returnDomain;
            }
            return null;
        }

        public IDecl? TryVisitDomainNameNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "domain") &&
                DoesContentContainNLooseChildren(node, "domain", 1))
            {
                var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "domain");
                return new DomainNameDecl(node, parent, name);
            }
            return null;
        }

        public IDecl? TryVisitRequirementsNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":requirements"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":requirements");
                var newReq = new RequirementsDecl(node, parent, new List<NameExp>());
                newReq.Requirements = ParseAsParameters(node, newReq, ":requirements", str);

                return newReq;
            }
            return null;
        }

        public IDecl? TryVisitExtendsNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":extends"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":extends");
                var newExt = new ExtendsDecl(node, parent, new List<NameExp>());
                newExt.Extends = ParseAsParameters(node, newExt, ":extends", str);

                return newExt;
            }
            return null;
        }

        public IDecl? TryVisitTypesNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":types"))
            {
                var newTypesDecl = new TypesDecl(node, parent, new List<TypeExp>());

                var str = RemoveNodeType(node.InnerContent, ":types").Trim();

                // In case types are decleared but is empty
                if (str == "")
                    return newTypesDecl;

                // Initial parse
                List<TypeExp> typeExps = new List<TypeExp>();
                str = str.Replace(ASTTokens.BreakToken, ' ').Trim();
                var typesDefSplit = str.Split(' ').ToList();
                typesDefSplit.Reverse();
                typesDefSplit.RemoveAll(x => x.Trim() == "");

                string currentSuperType = "";
                foreach (var typeDef in typesDefSplit)
                {
                    string newType = "";
                    if (typeDef.Contains(ASTTokens.TypeToken))
                    {
                        var split = typeDef.Split(ASTTokens.TypeToken).ToList();
                        split.RemoveAll(x => x.Trim() == "");

                        if (split.Count == 1)
                        {
                            currentSuperType = split[0].Trim();
                        }
                        else if (split.Count == 2)
                        {
                            currentSuperType = split[1].Trim();
                            newType = split[0].Trim();
                        }
                    }
                    else
                        newType = typeDef;
                    if (newType != "")
                        typeExps.Insert(0, new TypeExp(node, newTypesDecl, newType, currentSuperType, new HashSet<string>() { currentSuperType }));
                }

                // Stitch type inheritence
                if (typeExps.Count > 0)
                {
                    bool updatesFound = true;
                    while (updatesFound)
                    {
                        updatesFound = false;
                        foreach (var typeExp in typeExps)
                        {
                            foreach (var otherTypeExp in typeExps)
                            {
                                if (typeExp.Name != otherTypeExp.Name)
                                {
                                    if (typeExp.SuperTypes.Contains(otherTypeExp.Name))
                                    {
                                        var size = typeExp.SuperTypes.Count;
                                        typeExp.SuperTypes.AddRange(otherTypeExp.SuperTypes);
                                        if (size != typeExp.SuperTypes.Count)
                                            updatesFound = true;
                                    }
                                }
                            }
                        }
                    }
                }

                newTypesDecl.Types = typeExps;

                return newTypesDecl;
            }
            return null;
        }

        public IDecl? TryVisitConstantsNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":constants"))
            {
                var newCons = new ConstantsDecl(node, parent, new List<NameExp>());
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":constants");
                newCons.Constants = ParseAsParameters(node, newCons, ":constants", str);

                return newCons;
            }
            return null;
        }

        public IDecl? TryVisitPredicatesNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":predicates"))
            {
                var newPred = new PredicatesDecl(node, parent, new List<PredicateExp>());
                newPred.Predicates = ParseAsList<PredicateExp>(node, newPred);

                return newPred;
            }
            return null;
        }

        public IDecl? TryVisitFunctionsNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":functions"))
            {
                var newFuncs = new FunctionsDecl(node, parent, new List<PredicateExp>());
                newFuncs.Functions = ParseAsList<PredicateExp>(node, newFuncs);

                return newFuncs;
            }
            return null;
        }

        public IDecl? TryVisitTimelessNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":timeless"))
            {
                var newTime = new TimelessDecl(node, parent, new List<PredicateExp>());
                newTime.Items = ParseAsList<PredicateExp>(node, newTime);

                return newTime;
            }
            return null;
        }

        public IDecl? TryVisitActionNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":action") &&
                DoesContentContainTarget(node, ":action", ":parameters") &&
                DoesContentContainTarget(node, ":action", ":precondition") &&
                DoesContentContainTarget(node, ":action", ":effect") &&
                DoesNodeHaveSpecificChildCount(node, ":action", 3) &&
                DoesContentContainNLooseChildren(node, ":action", 4))
            {
                var nameFindStr = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, ":action"));
                var actionName = nameFindStr.Split(' ')[0].Trim();

                var newActionDecl = new ActionDecl(node, parent, actionName, null, null, null);

                // Parameters
                newActionDecl.Parameters = new ParameterExp(
                    node.Children[0],
                    newActionDecl,
                    ParseAsParameters(node.Children[0], newActionDecl, "parameters", node.Children[0].InnerContent));

                // Preconditions
                newActionDecl.Preconditions = VisitExp(node.Children[1], newActionDecl);

                // Effects
                newActionDecl.Effects = VisitExp(node.Children[2], newActionDecl);

                return newActionDecl;
            }
            return null;
        }

        public IDecl? TryVisitDurativeActionNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":durative-action") &&
                DoesContentContainTarget(node, ":durative-action", ":parameters") &&
                DoesContentContainTarget(node, ":durative-action", ":condition") &&
                DoesContentContainTarget(node, ":durative-action", ":duration") &&
                DoesContentContainTarget(node, ":durative-action", ":effect") &&
                DoesNodeHaveSpecificChildCount(node, ":durative-action", 4) &&
                DoesContentContainNLooseChildren(node, ":durative-action", 5))
            {
                var nameFindStr = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, ":durative-action"));
                var actionName = nameFindStr.Split(' ')[0].Trim();

                var newActionDecl = new DurativeActionDecl(node, parent, actionName, null, null, null, null);

                // Parameters
                newActionDecl.Parameters = new ParameterExp(
                    node.Children[0],
                    newActionDecl,
                    ParseAsParameters(node.Children[0], newActionDecl, "parameters", node.Children[0].InnerContent));

                // Duration
                newActionDecl.Duration = VisitExp(node.Children[1], newActionDecl);

                // Preconditions
                newActionDecl.Condition = VisitExp(node.Children[2], newActionDecl);

                // Effects
                newActionDecl.Effects = VisitExp(node.Children[3], newActionDecl);

                return newActionDecl;
            }
            return null;
        }

        public IDecl? TryVisitAxiomNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":axiom") &&
                DoesContentContainTarget(node, ":axiom", ":vars") &&
                DoesContentContainTarget(node, ":axiom", ":context") &&
                DoesContentContainTarget(node, ":axiom", ":implies") &&
                DoesNodeHaveSpecificChildCount(node, ":axiom", 3) &&
                DoesContentContainNLooseChildren(node, ":axiom", 3))
            {
                var newAxiomDecl = new AxiomDecl(node, parent, null, null, null);

                // Vars
                newAxiomDecl.Vars = new ParameterExp(
                    node.Children[0],
                    newAxiomDecl,
                    ParseAsParameters(node.Children[0], newAxiomDecl, "parameters", node.Children[0].InnerContent.Trim()));

                // Context
                newAxiomDecl.Context = VisitExp(node.Children[1], newAxiomDecl);

                // Implies
                newAxiomDecl.Implies = VisitExp(node.Children[2], newAxiomDecl);

                return newAxiomDecl;
            }
            return null;
        }

        public IDecl? TryVisitDerivedNode(ASTNode node, INode? parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":derived") &&
                DoesNodeHaveSpecificChildCount(node, ":derived", 2))
            {
                var derivedDecl = new DerivedDecl(node, parent, null, null);

                // Predicate
                derivedDecl.Predicate = TryVisitAs<PredicateExp>(node.Children[0], derivedDecl);

                // Expression
                derivedDecl.Expression = VisitExp(node.Children[1], derivedDecl);

                return derivedDecl;
            }
            return null;
        }
    }
}
