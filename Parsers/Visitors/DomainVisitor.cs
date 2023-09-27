﻿using PDDLSharp.ASTGenerators;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Tools;
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
        public IDecl VisitDomain(ASTNode node, INode parent)
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

            Listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing));
            return returnNode;
        }

        public IDecl? TryVisitDomainDeclNode(ASTNode node, INode parent)
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
                    }
                }
                return returnDomain;
            }
            return null;
        }

        public IDecl? TryVisitDomainNameNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, "domain") &&
                DoesContentContainNLooseChildren(node, "domain", 1))
            {
                var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "domain");
                return new DomainNameDecl(node, parent, name);
            }
            return null;
        }

        public IDecl? TryVisitRequirementsNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":requirements"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":requirements");
                var newReq = new RequirementsDecl(node, parent, new List<NameExp>());
                newReq.Requirements = LooseParseString<NameExp>(node, newReq, ":requirements", str);

                return newReq;
            }
            return null;
        }

        public IDecl? TryVisitExtendsNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":extends"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":extends");
                var newExt = new ExtendsDecl(node, parent, new List<NameExp>());
                newExt.Extends = LooseParseString<NameExp>(node, newExt, ":extends", str);

                return newExt;
            }
            return null;
        }

        public IDecl? TryVisitTypesNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":types"))
            {
                var newTypesDecl = new TypesDecl(node, parent, new List<TypeExp>());

                var str = ReplaceRangeWithSpaces(node.InnerContent, node.InnerContent.IndexOf(":types"), ":types".Length);

                if (str.Trim() == "")
                {
                    return newTypesDecl;
                }

                if (str.Contains(ASTTokens.TypeToken))
                {
                    var newTypes = new List<TypeExp>();

                    // Stitch type lines for inheritence
                    List<string> lines = new List<string>();
                    var splits = str.Split(ASTTokens.BreakToken);
                    string addLine = "";
                    foreach (var split in splits)
                    {
                        addLine += split;
                        if (addLine.Contains(ASTTokens.TypeToken))
                        {
                            lines.Add(addLine);
                            addLine = "";
                        }
                    }

                    int indexOffset = 0;
                    int characterOffset = node.Start + 1;
                    foreach (var line in lines)
                    {
                        characterOffset += line.Length + 1;
                        if (line != "")
                        {
                            int innerCharacterOffset = characterOffset;
                            int typesAdded = 0;
                            HashSet<string> superTypes = new HashSet<string>();
                            var superTypeStr = line.Substring(line.LastIndexOf(ASTTokens.TypeToken) + ASTTokens.TypeToken.Length);
                            if (superTypeStr.Contains(ASTTokens.TypeToken) || superTypeStr.Trim().Contains(' '))
                                Listener.AddError(new ParseError(
                                    "Type definition cannot have two supertypes!",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Parsing
                                    ));

                            var superType = superTypeStr.Trim();
                            superTypes.Add(superType);
                            var newType = new TypeExp(
                                new ASTNode(innerCharacterOffset - superType.Length, innerCharacterOffset, node.Line),
                                newTypesDecl,
                                superType, new HashSet<string>());
                            newTypes.Insert(indexOffset, newType);
                            innerCharacterOffset -= superType.Length + ASTTokens.TypeToken.Length;
                            typesAdded++;

                            var subTypesStr = line.Substring(0, line.IndexOf(ASTTokens.TypeToken));
                            foreach (var param in subTypesStr.Split(' ').Reverse())
                            {
                                if (param != "")
                                {
                                    var newSubType = new TypeExp(
                                        new ASTNode(innerCharacterOffset - param.Length, innerCharacterOffset, node.Line),
                                        newTypesDecl, param, superTypes);
                                    newTypes.Insert(indexOffset, newSubType);
                                    innerCharacterOffset -= param.Length + 1;
                                    typesAdded++;
                                }
                            }

                            indexOffset += typesAdded;
                        }
                    }

                    foreach (var type in newTypes)
                    {
                        if (!newTypesDecl.Types.Any(x => x.Name == type.Name))
                        {
                            var all = newTypes.FindAll(x => x.Name == type.Name);
                            if (all.Count > 1)
                            {
                                HashSet<string> merged = new HashSet<string>();
                                foreach (var copyType in all)
                                    merged.AddRange(copyType.SuperTypes);
                                newTypesDecl.Types.Add(
                                    new TypeExp(new ASTNode(type.Start, type.End, type.Line),
                                    newTypesDecl,
                                    type.Name,
                                    merged));
                            }
                            else
                                newTypesDecl.Types.Add(type);
                        }
                    }
                }
                else
                {
                    int characterOffset = node.End - 1;
                    str = ReduceToSingleSpace(PurgeEscapeChars(str));
                    foreach (var param in str.Split(' ').Reverse())
                    {
                        if (param != "")
                        {
                            var newSubType = new TypeExp(
                                new ASTNode(characterOffset - param.Length, characterOffset, node.Line),
                                newTypesDecl, param, new HashSet<string>());
                            newTypesDecl.Types.Insert(0, newSubType);
                            characterOffset -= param.Length + 1;
                        }
                    }
                }

                return newTypesDecl;
            }
            return null;
        }

        public IDecl? TryVisitConstantsNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":constants"))
            {
                var newCons = new ConstantsDecl(node, parent, new List<NameExp>());
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":constants");
                newCons.Constants = LooseParseString<NameExp>(node, newCons, ":constants", str);

                return newCons;
            }
            return null;
        }

        public IDecl? TryVisitPredicatesNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":predicates"))
            {
                var newPred = new PredicatesDecl(node, parent, new List<PredicateExp>());
                newPred.Predicates = ParseAsList<PredicateExp>(node, newPred);

                return newPred;
            }
            return null;
        }

        public IDecl? TryVisitFunctionsNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":functions"))
            {
                var newFuncs = new FunctionsDecl(node, parent, new List<PredicateExp>());
                newFuncs.Functions = ParseAsList<PredicateExp>(node, newFuncs);

                return newFuncs;
            }
            return null;
        }

        public IDecl? TryVisitTimelessNode(ASTNode node, INode parent)
        {
            if (IsOfValidNodeType(node.InnerContent, ":timeless"))
            {
                var newTime = new TimelessDecl(node, parent, new List<PredicateExp>());
                newTime.Items = ParseAsList<PredicateExp>(node, newTime);

                return newTime;
            }
            return null;
        }

        public IDecl? TryVisitActionNode(ASTNode node, INode parent)
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
                newActionDecl.Parameters = new ParameterDecl(
                    node.Children[0],
                    newActionDecl,
                    LooseParseString<NameExp>(node.Children[0], newActionDecl, "parameters", node.Children[0].InnerContent));

                // Preconditions
                newActionDecl.Preconditions = VisitExp(node.Children[1], newActionDecl);

                // Effects
                newActionDecl.Effects = VisitExp(node.Children[2], newActionDecl);

                return newActionDecl;
            }
            return null;
        }

        public IDecl? TryVisitDurativeActionNode(ASTNode node, INode parent)
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
                newActionDecl.Parameters = new ParameterDecl(
                    node.Children[0],
                    newActionDecl,
                    LooseParseString<NameExp>(node.Children[0], newActionDecl, "parameters", node.Children[0].InnerContent));

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

        public IDecl? TryVisitAxiomNode(ASTNode node, INode parent)
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
                newAxiomDecl.Vars = new ParameterDecl(
                    node.Children[0],
                    newAxiomDecl,
                    LooseParseString<NameExp>(node.Children[0], newAxiomDecl, "parameters", node.Children[0].InnerContent.Trim()));

                // Context
                newAxiomDecl.Context = VisitExp(node.Children[1], newAxiomDecl);

                // Implies
                newAxiomDecl.Implies = VisitExp(node.Children[2], newAxiomDecl);

                return newAxiomDecl;
            }
            return null;
        }
    }
}
