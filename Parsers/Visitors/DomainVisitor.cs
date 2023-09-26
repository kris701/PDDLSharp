using ASTGenerator;
using ErrorListeners;
using PDDLModels;
using PDDLModels.AST;
using PDDLModels.Domain;
using Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Parsers.Visitors
{
    public class DomainVisitor : BaseVisitor, IVisitor<ASTNode, INode, IDecl>
    {
        public IDecl Visit(ASTNode node, INode parent, IErrorListener listener)
        {
            IDecl returnNode = null;
            if (TryVisitDomainDeclNode(node, parent, listener, out returnNode))
                return returnNode;
            if (TryVisitDomainNameNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitRequirementsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitExtendsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitTypesNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitConstantsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitPredicatesNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitFunctionsNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitTimelessNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitActionNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitDurativeActionNode(node, parent, listener, out returnNode))
                return returnNode;
            else if (TryVisitAxiomNode(node, parent, listener, out returnNode))
                return returnNode;

            listener.AddError(new ParseError(
                $"Could not parse content of AST node: {node.OuterContent}",
                ParseErrorType.Error,
                ParseErrorLevel.Parsing,
                ParserErrorCode.UnknownNode));
            return default;
        }

        public bool TryVisitDomainDeclNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "define"))
            {
                if (DoesNotContainStrayCharacters(node, "define", listener))
                {
                    var returnDomain = new DomainDecl(node);
                    foreach (var child in node.Children)
                    {
                        var visited = Visit(child, returnDomain, listener);
                        if (visited is DomainNameDecl domainName)
                            returnDomain.Name = domainName;
                        else if (visited is RequirementsDecl requirements)
                            returnDomain.Requirements = requirements;
                        else if (visited is ExtendsDecl extends)
                            returnDomain.Extends = extends;
                        else if (visited is TypesDecl types)
                            returnDomain.Types = types;
                        else if (visited is ConstantsDecl constants)
                            returnDomain.Constants = constants;
                        else if (visited is TimelessDecl timeless)
                            returnDomain.Timeless = timeless;
                        else if (visited is PredicatesDecl predicates)
                            returnDomain.Predicates = predicates;
                        else if (visited is FunctionsDecl funcs)
                            returnDomain.Functions = funcs;
                        else if (visited is ActionDecl act)
                        {
                            if (returnDomain.Actions == null)
                                returnDomain.Actions = new List<ActionDecl>();
                            returnDomain.Actions.Add(act);
                        }
                        else if (visited is DurativeActionDecl dAct)
                        {
                            if (returnDomain.DurativeActions == null)
                                returnDomain.DurativeActions = new List<DurativeActionDecl>();
                            returnDomain.DurativeActions.Add(dAct);
                        }
                        else if (visited is AxiomDecl axi)
                        {
                            if (returnDomain.Axioms == null)
                                returnDomain.Axioms = new List<AxiomDecl>();
                            returnDomain.Axioms.Add(axi);
                        }
                    }
                    decl = returnDomain;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitDomainNameNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, "domain"))
            {
                if (DoesContentContainNLooseChildren(node, "domain", 1, listener))
                {
                    var name = RemoveNodeTypeAndEscapeChars(node.InnerContent, "domain");
                    decl = new DomainNameDecl(node, parent, name);
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitRequirementsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":requirements"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":requirements");
                var newReq = new RequirementsDecl(node, parent, new List<NameExp>());
                newReq.Requirements = LooseParseString<NameExp>(node, newReq, ":requirements", str, listener);

                decl = newReq;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitExtendsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":extends"))
            {
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":extends");
                var newExt = new ExtendsDecl(node, parent, new List<NameExp>());
                newExt.Extends = LooseParseString<NameExp>(node, newExt, ":extends", str, listener);

                decl = newExt;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitTypesNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":types"))
            {
                var newTypesDecl = new TypesDecl(node, parent, new List<TypeExp>());

                var str = ReplaceRangeWithSpaces(node.InnerContent, node.InnerContent.IndexOf(":types"), ":types".Length);

                if (str.Trim() == "")
                {
                    decl = newTypesDecl;
                    return true;
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
                                listener.AddError(new ParseError(
                                    "Type definition cannot have two supertypes!",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Parsing,
                                    ParserErrorCode.TypeDeclarationError
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

                decl = newTypesDecl;
                return true;
            }
            decl = null;
            return false;
        }

        private string ReplaceRangeWithSpaces(string text, int from, int to)
        {
            var newText = text.Substring(0, from);
            newText += new string(' ', to - from);
            newText += text.Substring(to);
            return newText;
        }

        public bool TryVisitConstantsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":constants"))
            {
                var newCons = new ConstantsDecl(node, parent, new List<NameExp>());
                var str = RemoveNodeTypeAndEscapeChars(node.InnerContent, ":constants");
                newCons.Constants = LooseParseString<NameExp>(node, newCons, ":constants", str, listener);

                decl = newCons;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitPredicatesNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":predicates"))
            {
                var newPred = new PredicatesDecl(node, parent, new List<PredicateExp>());
                newPred.Predicates = ParseAsList<PredicateExp>(node, newPred, listener);

                decl = newPred;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitFunctionsNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":functions"))
            {
                var newFuncs = new FunctionsDecl(node, parent, new List<PredicateExp>());
                newFuncs.Functions = ParseAsList<PredicateExp>(node, newFuncs, listener);

                decl = newFuncs;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitTimelessNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":timeless"))
            {
                var newTime = new TimelessDecl(node, parent, new List<PredicateExp>());
                newTime.Items = ParseAsList<PredicateExp>(node, newTime, listener);

                decl = newTime;
                return true;
            }
            decl = null;
            return false;
        }

        public bool TryVisitActionNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":action"))
            {
                if (DoesContentContainTarget(node, ":action", ":parameters", listener) &&
                    DoesContentContainTarget(node, ":action", ":precondition", listener) &&
                    DoesContentContainTarget(node, ":action", ":effect", listener) &&
                    DoesNodeHaveSpecificChildCount(node, ":action", 3, listener) &&
                    DoesContentContainNLooseChildren(node, ":action", 4, listener))
                {

                    var nameFindStr = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, ":action"));
                    var actionName = nameFindStr.Split(' ')[0].Trim();

                    var newActionDecl = new ActionDecl(node, parent, actionName, null, null, null);
                    var visitor = new ExpVisitor();

                    // Parameters
                    newActionDecl.Parameters = new ParameterDecl(
                        node.Children[0],
                        newActionDecl,
                        LooseParseString<NameExp>(node.Children[0], newActionDecl, "parameters", node.Children[0].InnerContent, listener));

                    // Preconditions
                    newActionDecl.Preconditions = visitor.Visit(node.Children[1], newActionDecl, listener);

                    // Effects
                    newActionDecl.Effects = visitor.Visit(node.Children[2], newActionDecl, listener);

                    decl = newActionDecl;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitDurativeActionNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":durative-action"))
            {
                if (DoesContentContainTarget(node, ":durative-action", ":parameters", listener) &&
                    DoesContentContainTarget(node, ":durative-action", ":condition", listener) &&
                    DoesContentContainTarget(node, ":durative-action", ":duration", listener) &&
                    DoesContentContainTarget(node, ":durative-action", ":effect", listener) &&
                    DoesNodeHaveSpecificChildCount(node, ":durative-action", 4, listener) &&
                    DoesContentContainNLooseChildren(node, ":durative-action", 5, listener))
                {

                    var nameFindStr = ReduceToSingleSpace(RemoveNodeTypeAndEscapeChars(node.InnerContent, ":durative-action"));
                    var actionName = nameFindStr.Split(' ')[0].Trim();

                    var newActionDecl = new DurativeActionDecl(node, parent, actionName, null, null, null, null);
                    var visitor = new ExpVisitor();

                    // Parameters
                    newActionDecl.Parameters = new ParameterDecl(
                        node.Children[0],
                        newActionDecl,
                        LooseParseString<NameExp>(node.Children[0], newActionDecl, "parameters", node.Children[0].InnerContent, listener));

                    // Duration
                    newActionDecl.Duration = visitor.Visit(node.Children[1], newActionDecl, listener);

                    // Preconditions
                    newActionDecl.Condition = visitor.Visit(node.Children[2], newActionDecl, listener);

                    // Effects
                    newActionDecl.Effects = visitor.Visit(node.Children[3], newActionDecl, listener);

                    decl = newActionDecl;
                    return true;
                }
            }
            decl = null;
            return false;
        }

        public bool TryVisitAxiomNode(ASTNode node, INode parent, IErrorListener listener, out IDecl decl)
        {
            if (IsOfValidNodeType(node.InnerContent, ":axiom"))
            {
                if (DoesContentContainTarget(node, ":axiom", ":vars", listener) &&
                    DoesContentContainTarget(node, ":axiom", ":context", listener) &&
                    DoesContentContainTarget(node, ":axiom", ":implies", listener) &&
                    DoesNodeHaveSpecificChildCount(node, ":axiom", 3, listener) &&
                    DoesContentContainNLooseChildren(node, ":axiom", 3, listener))
                {

                    var newAxiomDecl = new AxiomDecl(node, parent, null, null, null);
                    var visitor = new ExpVisitor();

                    // Vars
                    newAxiomDecl.Vars = new ParameterDecl(
                        node.Children[0],
                        newAxiomDecl,
                        LooseParseString<NameExp>(node.Children[0], newAxiomDecl, "parameters", node.Children[0].InnerContent.Trim(), listener));

                    // Context
                    newAxiomDecl.Context = visitor.Visit(node.Children[1], newAxiomDecl, listener);

                    // Implies
                    newAxiomDecl.Implies = visitor.Visit(node.Children[2], newAxiomDecl, listener);

                    decl = newAxiomDecl;
                    return true;
                }
            }
            decl = null;
            return false;
        }
    }
}
