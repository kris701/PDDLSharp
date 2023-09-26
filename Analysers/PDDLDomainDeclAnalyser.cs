using ErrorListeners;
using PDDLModels;
using PDDLModels.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace PDDLParser.Analysers
{
    public class PDDLDomainDeclAnalyser : IAnalyser<DomainDecl>
    {
        public void PostAnalyse(DomainDecl decl, IErrorListener listener)
        {
            // Basics
            CheckForBasicDomain(decl, listener);

            // Declare Checking
            CheckForUndeclaredPredicates(decl, listener);
            CheckForUnusedPredicates(decl, listener);
            CheckForUnusedTypes(decl, listener);
            CheckForUnusedActionParameters(decl, listener);
            CheckForUnusedAxiomParameters(decl, listener);

            // Types
            CheckForValidTypesInPredicates(decl, listener);
            CheckForValidTypesInConstants(decl, listener);
            CheckTypeMatchForActions(decl, listener);
            CheckTypeMatchForAxioms(decl, listener);

            // Unique Name Checking
            CheckForUniquePredicateNames(decl, listener);
            CheckForUniquePredicateParameterNames(decl, listener);
            CheckForUniqueActionNames(decl, listener);
            CheckForUniqueActionParameterNames(decl, listener);
            CheckForUniqueAxiomParameterNames(decl, listener);

            // Validity Checking
            CheckActionUsesValidPredicates(decl, listener);
            CheckAxiomUsesValidPredicates(decl, listener);
        }

        public void PreAnalyse(string text, IErrorListener listener)
        {
            throw new NotImplementedException();
        }

        private void CheckForBasicDomain(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates == null)
                listener.AddError(new ParseError(
                    $"Missing predicates declaration.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
            if (domain.Predicates != null && domain.Predicates.Predicates.Count == 0)
                listener.AddError(new ParseError(
                    $"No predicates defined.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
            if (domain.Actions == null)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
            if (domain.Actions != null && domain.Actions.Count == 0)
                listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    ParserErrorCode.MissingItem,
                    domain.Line,
                    domain.Start));
        }

        private void CheckForUndeclaredPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                List<string> simplePredNames = new List<string>();
                foreach (var pred in domain.Predicates.Predicates)
                    simplePredNames.Add(pred.Name);
                if (domain.Functions != null)
                    foreach (var func in domain.Functions.Functions)
                        simplePredNames.Add(func.Name);
                var allPreds = domain.FindTypes<PredicateExp>();
                foreach(var pred in allPreds)
                {
                    if (!simplePredNames.Contains(pred.Name))
                        listener.AddError(new ParseError(
                            $"Undefined predicate! '{pred}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UseOfUndeclaredPredicate,
                            pred.Line,
                            pred.Start));
                }
            }
        }
        private void CheckForUnusedPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    if (domain.FindNames(predicate.Name).Count == 1)
                        listener.AddError(new ParseError(
                            $"Unused predicate detected '{predicate}'",
                            ParseErrorType.Message,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UnusedPredicate,
                            predicate.Line,
                            predicate.Start));
                }
            }
        }
        private void CheckForUnusedTypes(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Types != null)
            {
                var allTypes = domain.FindTypes<TypeExp>();
                foreach (var type in domain.Types.Types)
                {
                    if (allTypes.Count(x => x.Name == type.Name) == 1)
                        listener.AddError(new ParseError(
                            $"Unused type detected '{type}'",
                            ParseErrorType.Message,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UnusedPredicate,
                            type.Line,
                            type.Start));
                }
            }
        }
        private void CheckForUnusedActionParameters(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                foreach (var act in domain.Actions)
                {
                    foreach (var param in act.Parameters.Values)
                        if (act.FindNames(param.Name).Count == 0)
                            listener.AddError(new ParseError(
                                $"Unused action parameter found '{param.Name}'",
                                ParseErrorType.Message,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnusedParameter,
                                param.Line,
                                param.Start));
                }
            }
        }
        private void CheckForUnusedAxiomParameters(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null)
            {
                foreach (var axi in domain.Axioms)
                {
                    foreach (var param in axi.Vars.Values)
                        if (axi.FindNames(param.Name).Count == 0)
                            listener.AddError(new ParseError(
                                $"Unused axiom variable found '{param.Name}'",
                                ParseErrorType.Message,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnusedParameter,
                                param.Line,
                                param.Start));
                }
            }
        }

        private void CheckForValidTypesInPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                foreach(var predicate in domain.Predicates.Predicates)
                {
                    foreach(var arg in predicate.Arguments)
                    {
                        if (!domain.ContainsType(arg.Type.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Predicate arguments contains unknown type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnknownType,
                                arg.Line,
                                arg.Start));
                        }
                    }
                }
            }
        }
        private void CheckTypeMatchForActions(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                foreach(var act in domain.Actions)
                {
                    foreach(var param in act.Parameters.Values)
                    {
                        if (!domain.ContainsType(param.Type.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnknownType,
                                param.Line,
                                param.Start));
                        }
                    }

                    CheckForValidTypesInAction(act.Preconditions, domain, act, listener);
                    CheckForValidTypesInAction(act.Effects, domain, act, listener);
                }
            }
        }
        private void CheckForValidTypesInAction(IExp node, DomainDecl domain, ActionDecl action, IErrorListener listener)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForValidTypesInAction(child, domain, action, listener);
            }
            else if (node is OrExp or)
            {
                CheckForValidTypesInAction(or.Option1, domain, action, listener);
                CheckForValidTypesInAction(or.Option2, domain, action, listener);
            }
            else if (node is NotExp not)
            {
                CheckForValidTypesInAction(not.Child, domain, action, listener);
            }
            else if (node is PredicateExp pred)
            {
                int index = 0;
                foreach (var arg in pred.Arguments)
                {
                    var argOrCons = action.GetParameterOrConstant(arg.Name);
                    if (argOrCons == null)
                    {
                        listener.AddError(new ParseError(
                            $"Arguments does not match the predicate definition!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.InvalidPredicateType,
                            arg.Line,
                            arg.Start));
                    }
                    else if (argOrCons.Name != arg.Name && !arg.Type.IsTypeOf(argOrCons.Type.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Predicate has an invalid argument type! Expected a '{action.GetParameterOrConstant(arg.Name).Name}' but got a '{arg.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.TypeMissmatch,
                            arg.Line,
                            arg.Start));
                    }
                    index++;
                }
            }
        }
        private void CheckTypeMatchForAxioms(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null)
            {
                foreach (var axi in domain.Axioms)
                {
                    foreach (var param in axi.Vars.Values)
                    {
                        if (!domain.ContainsType(param.Type.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UnknownType,
                                param.Line,
                                param.Start));
                        }
                    }

                    CheckForValidTypesInAxiom(axi.Context, domain, axi, listener);
                    CheckForValidTypesInAxiom(axi.Implies, domain, axi, listener);
                }
            }
        }
        private void CheckForValidTypesInAxiom(IExp node, DomainDecl domain, AxiomDecl axiom, IErrorListener listener)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForValidTypesInAxiom(child, domain, axiom, listener);
            }
            else if (node is OrExp or)
            {
                CheckForValidTypesInAxiom(or.Option1, domain, axiom, listener);
                CheckForValidTypesInAxiom(or.Option2, domain, axiom, listener);
            }
            else if (node is NotExp not)
            {
                CheckForValidTypesInAxiom(not.Child, domain, axiom, listener);
            }
            else if (node is PredicateExp pred)
            {
                int index = 0;
                foreach (var arg in pred.Arguments)
                {
                    if (!arg.Type.IsTypeOf(axiom.GetParameterOrConstant(arg.Name).Type.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Predicate has an invalid argument type! Expected a '{axiom.GetParameterOrConstant(arg.Name).Name}' but got a '{arg.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.TypeMissmatch,
                            arg.Line,
                            arg.Start));
                    }
                    index++;
                }
            }
        }

        private void CheckForValidTypesInConstants(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Constants != null)
            {
                foreach(var cons in domain.Constants.Constants)
                {
                    if (!domain.ContainsType(cons.Type.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Constant contains unknown type!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UnknownType,
                            cons.Line,
                            cons.Start));
                    }
                }
            }
        }

        private void CheckForUniquePredicateNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                List<string> predicates = new List<string>();
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    if (predicates.Contains(predicate.Name))
                    {
                        listener.AddError(new ParseError(
                                $"Multiple declarations of predicates with the same name '{predicate.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.MultipleDeclarationsOfPredicate,
                                predicate.Line,
                                predicate.Start));
                    }
                    predicates.Add(predicate.Name);
                }
            }
        }
        private void CheckForUniquePredicateParameterNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Predicates != null)
            {
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    List<string> parameterNames = new List<string>();
                    foreach (var param in predicate.Arguments)
                    {
                        if (parameterNames.Contains(param.Name))
                        {
                            listener.AddError(new ParseError(
                                $"Multiple declarations of arguments with the same name '{param.Name}' in the predicate '{predicate.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.MultipleDeclarationsOfParameter,
                                param.Line,
                                param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private void CheckForUniqueActionNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                List<string> actions = new List<string>();
                foreach (var act in domain.Actions)
                {
                    if (actions.Contains(act.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Multiple declarations of actions with the same name '{act.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.MultipleDeclarationsOfActions,
                            act.Line,
                            act.Start));
                    }
                    actions.Add(act.Name);
                }
            }
        }
        private void CheckForUniqueActionParameterNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null)
            {
                foreach (var action in domain.Actions)
                {
                    List<string> parameterNames = new List<string>();
                    foreach (var param in action.Parameters.Values)
                    {
                        if (parameterNames.Contains(param.Name))
                        {
                            listener.AddError(new ParseError(
                                    $"Multiple declarations of arguments with the same name '{param.Name}' in the action '{action.Name}'",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Analyser,
                                    ParserErrorCode.MultipleDeclarationsOfParameter,
                                    param.Line,
                                    param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private void CheckForUniqueAxiomParameterNames(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null)
            {
                foreach (var axiom in domain.Axioms)
                {
                    List<string> parameterNames = new List<string>();
                    foreach (var param in axiom.Vars.Values)
                    {
                        if (parameterNames.Contains(param.Name))
                        {
                            listener.AddError(new ParseError(
                                    $"Multiple declarations of arguments with the same name '{param.Name}' in axiom",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Analyser,
                                    ParserErrorCode.MultipleDeclarationsOfParameter,
                                    param.Line,
                                    param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }

        private void CheckActionUsesValidPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Actions != null && domain.Predicates != null)
            {
                foreach (var action in domain.Actions)
                {
                    CheckExpUsesPredicates(action.Preconditions, domain.Predicates.Predicates, listener, domain);
                    CheckExpUsesPredicates(action.Effects, domain.Predicates.Predicates, listener, domain);
                }
            }
        }
        private void CheckAxiomUsesValidPredicates(DomainDecl domain, IErrorListener listener)
        {
            if (domain.Axioms != null && domain.Predicates != null)
            {
                foreach (var axiom in domain.Axioms)
                {
                    CheckExpUsesPredicates(axiom.Context, domain.Predicates.Predicates, listener, domain);
                    CheckExpUsesPredicates(axiom.Implies, domain.Predicates.Predicates, listener, domain);
                }
            }
        }
        private void CheckExpUsesPredicates(IExp node, List<PredicateExp> predicates, IErrorListener listener, DomainDecl domain)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckExpUsesPredicates(child, predicates, listener, domain);
            }
            else if (node is OrExp or)
            {
                CheckExpUsesPredicates(or.Option1, predicates, listener, domain);
                CheckExpUsesPredicates(or.Option2, predicates, listener, domain);
            }
            else if (node is NotExp not)
            {
                CheckExpUsesPredicates(not.Child, predicates, listener, domain);
            }
            else if (node is PredicateExp pred)
            {
                bool any = false;
                bool wasTypeMissmatch = false;
                foreach (var predicate in predicates)
                {
                    if (predicate.Name == pred.Name && predicate.Arguments.Count == pred.Arguments.Count)
                    {
                        any = true;
                        for (int i = 0; i < predicate.Arguments.Count; i++)
                        {
                            if (!pred.Arguments[i].Type.IsTypeOf(predicate.Arguments[i].Type.Name))
                            {
                                if (domain.Constants != null)
                                {
                                    if (domain.Constants.Constants.Any(x => x.Name == pred.Arguments[i].Name))
                                        continue;
                                }
                                wasTypeMissmatch = true;
                                any = false;
                                break;
                            }
                        }
                    }
                }
                if (!any)
                {
                    if (wasTypeMissmatch)
                        listener.AddError(new ParseError(
                            $"Used predicate '{pred.Name}' did not match the type definitions from the parameters!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.TypeMissmatch,
                            pred.Line,
                            pred.Start));
                    else
                        listener.AddError(new ParseError(
                            $"Undeclared predicate used '{pred.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UseOfUndeclaredPredicate,
                            pred.Line,
                            pred.Start));
                }
            }
        }
    }
}
