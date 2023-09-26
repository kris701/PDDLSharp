using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace PDDLSharp.Analysers
{
    public class PDDLDomainDeclAnalyser : IAnalyser<DomainDecl>
    {
        public IErrorListener Listener { get; }

        public PDDLDomainDeclAnalyser(IErrorListener listener)
        {
            Listener = listener;
        }

        public void PostAnalyse(DomainDecl decl)
        {
            // Basics
            CheckForBasicDomain(decl);

            // Declare Checking
            CheckForUndeclaredPredicates(decl);
            CheckForUnusedPredicates(decl);
            CheckForUnusedTypes(decl);
            CheckForUnusedActionParameters(decl);
            CheckForUnusedAxiomParameters(decl);

            // Types
            CheckForValidTypesInPredicates(decl);
            CheckForValidTypesInConstants(decl);
            CheckTypeMatchForActions(decl);
            CheckTypeMatchForAxioms(decl);

            // Unique Name Checking
            CheckForUniquePredicateNames(decl);
            CheckForUniquePredicateParameterNames(decl);
            CheckForUniqueActionNames(decl);
            CheckForUniqueActionParameterNames(decl);
            CheckForUniqueAxiomParameterNames(decl);

            // Validity Checking
            CheckActionUsesValidPredicates(decl);
            CheckAxiomUsesValidPredicates(decl);
        }

        public void PreAnalyse(string text)
        {
            throw new NotImplementedException();
        }

        private void CheckForBasicDomain(DomainDecl domain)
        {
            if (domain.Predicates == null)
                Listener.AddError(new ParseError(
                    $"Missing predicates declaration.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Predicates != null && domain.Predicates.Predicates.Count == 0)
                Listener.AddError(new ParseError(
                    $"No predicates defined.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Actions == null)
                Listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Actions != null && domain.Actions.Count == 0)
                Listener.AddError(new ParseError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
        }

        private void CheckForUndeclaredPredicates(DomainDecl domain)
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
                        Listener.AddError(new ParseError(
                            $"Undefined predicate! '{pred}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            pred.Line,
                            pred.Start));
                }
            }
        }
        private void CheckForUnusedPredicates(DomainDecl domain)
        {
            if (domain.Predicates != null)
            {
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    if (domain.FindNames(predicate.Name).Count == 1)
                        Listener.AddError(new ParseError(
                            $"Unused predicate detected '{predicate}'",
                            ParseErrorType.Message,
                            ParseErrorLevel.Analyser,
                            predicate.Line,
                            predicate.Start));
                }
            }
        }
        private void CheckForUnusedTypes(DomainDecl domain)
        {
            if (domain.Types != null)
            {
                var allTypes = domain.FindTypes<TypeExp>();
                foreach (var type in domain.Types.Types)
                {
                    if (allTypes.Count(x => x.Name == type.Name) == 1)
                        Listener.AddError(new ParseError(
                            $"Unused type detected '{type}'",
                            ParseErrorType.Message,
                            ParseErrorLevel.Analyser,
                            type.Line,
                            type.Start));
                }
            }
        }
        private void CheckForUnusedActionParameters(DomainDecl domain)
        {
            if (domain.Actions != null)
            {
                foreach (var act in domain.Actions)
                {
                    foreach (var param in act.Parameters.Values)
                        if (act.FindNames(param.Name).Count == 0)
                            Listener.AddError(new ParseError(
                                $"Unused action parameter found '{param.Name}'",
                                ParseErrorType.Message,
                                ParseErrorLevel.Analyser,
                                param.Line,
                                param.Start));
                }
            }
        }
        private void CheckForUnusedAxiomParameters(DomainDecl domain)
        {
            if (domain.Axioms != null)
            {
                foreach (var axi in domain.Axioms)
                {
                    foreach (var param in axi.Vars.Values)
                        if (axi.FindNames(param.Name).Count == 0)
                            Listener.AddError(new ParseError(
                                $"Unused axiom variable found '{param.Name}'",
                                ParseErrorType.Message,
                                ParseErrorLevel.Analyser,
                                param.Line,
                                param.Start));
                }
            }
        }

        private void CheckForValidTypesInPredicates(DomainDecl domain)
        {
            if (domain.Predicates != null)
            {
                foreach(var predicate in domain.Predicates.Predicates)
                {
                    foreach(var arg in predicate.Arguments)
                    {
                        if (!ContainsType(domain, arg.Type.Name))
                        {
                            Listener.AddError(new ParseError(
                                $"Predicate arguments contains unknown type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                arg.Line,
                                arg.Start));
                        }
                    }
                }
            }
        }
        public bool ContainsType(DomainDecl domain, string target)
        {
            if (target == "")
                return true;
            if (domain.Types == null)
                return false;
            foreach (var type in domain.Types.Types)
            {
                if (type.IsTypeOf(target))
                    return true;
            }
            return false;
        }
        private void CheckTypeMatchForActions(DomainDecl domain)
        {
            if (domain.Actions != null)
            {
                foreach(var act in domain.Actions)
                {
                    foreach(var param in act.Parameters.Values)
                    {
                        if (!ContainsType(domain, param.Type.Name))
                        {
                            Listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                param.Line,
                                param.Start));
                        }
                    }

                    CheckForValidTypesInAction(act.Preconditions, domain, act);
                    CheckForValidTypesInAction(act.Effects, domain, act);
                }
            }
        }
        private void CheckForValidTypesInAction(IExp node, DomainDecl domain, ActionDecl action)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForValidTypesInAction(child, domain, action);
            }
            else if (node is OrExp or)
            {
                CheckForValidTypesInAction(or.Option1, domain, action);
                CheckForValidTypesInAction(or.Option2, domain, action);
            }
            else if (node is NotExp not)
            {
                CheckForValidTypesInAction(not.Child, domain, action);
            }
            else if (node is PredicateExp pred)
            {
                int index = 0;
                foreach (var arg in pred.Arguments)
                {
                    var argOrCons = GetParameterOrConstant(action, arg.Name);
                    if (argOrCons == null)
                    {
                        Listener.AddError(new ParseError(
                            $"Arguments does not match the predicate definition!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            arg.Line,
                            arg.Start));
                    }
                    else if (argOrCons.Name != arg.Name && !arg.Type.IsTypeOf(argOrCons.Type.Name))
                    {
                        Listener.AddError(new ParseError(
                            $"Predicate has an invalid argument type! Expected a '{GetParameterOrConstant(action, arg.Name).Name}' but got a '{arg.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            arg.Line,
                            arg.Start));
                    }
                    index++;
                }
            }
        }
        private NameExp GetParameterOrConstant(ActionDecl action, string name)
        {
            var concrete = action.Parameters.Values.SingleOrDefault(x => x.Name == name);
            if (concrete == null)
                if (action.Parent is DomainDecl domain)
                    if (domain.Constants != null)
                        return domain.Constants.Constants.SingleOrDefault(x => x.Name == name);
            return concrete;
        }
        private void CheckTypeMatchForAxioms(DomainDecl domain)
        {
            if (domain.Axioms != null)
            {
                foreach (var axi in domain.Axioms)
                {
                    foreach (var param in axi.Vars.Values)
                    {
                        if (!ContainsType(domain, param.Type.Name))
                        {
                            Listener.AddError(new ParseError(
                                $"Parameter contains unknow type!",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                param.Line,
                                param.Start));
                        }
                    }

                    CheckForValidTypesInAxiom(axi.Context, domain, axi);
                    CheckForValidTypesInAxiom(axi.Implies, domain, axi);
                }
            }
        }
        private void CheckForValidTypesInAxiom(IExp node, DomainDecl domain, AxiomDecl axiom)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForValidTypesInAxiom(child, domain, axiom);
            }
            else if (node is OrExp or)
            {
                CheckForValidTypesInAxiom(or.Option1, domain, axiom);
                CheckForValidTypesInAxiom(or.Option2, domain, axiom);
            }
            else if (node is NotExp not)
            {
                CheckForValidTypesInAxiom(not.Child, domain, axiom);
            }
            else if (node is PredicateExp pred)
            {
                int index = 0;
                foreach (var arg in pred.Arguments)
                {
                    if (!arg.Type.IsTypeOf(GetParameterOrConstant(axiom, arg.Name).Type.Name))
                    {
                        Listener.AddError(new ParseError(
                            $"Predicate has an invalid argument type! Expected a '{GetParameterOrConstant(axiom, arg.Name).Name}' but got a '{arg.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            arg.Line,
                            arg.Start));
                    }
                    index++;
                }
            }
        }
        public NameExp GetParameterOrConstant(AxiomDecl axi, string name)
        {
            var concrete = axi.Vars.Values.SingleOrDefault(x => x.Name == name);
            if (concrete == null)
                if (axi.Parent is DomainDecl domain)
                    if (domain.Constants != null)
                        return domain.Constants.Constants.SingleOrDefault(x => x.Name == name);
            return concrete;
        }
        private void CheckForValidTypesInConstants(DomainDecl domain)
        {
            if (domain.Constants != null)
            {
                foreach(var cons in domain.Constants.Constants)
                {
                    if (!ContainsType(domain, cons.Type.Name))
                    {
                        Listener.AddError(new ParseError(
                            $"Constant contains unknown type!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            cons.Line,
                            cons.Start));
                    }
                }
            }
        }

        private void CheckForUniquePredicateNames(DomainDecl domain)
        {
            if (domain.Predicates != null)
            {
                List<string> predicates = new List<string>();
                foreach (var predicate in domain.Predicates.Predicates)
                {
                    if (predicates.Contains(predicate.Name))
                    {
                        Listener.AddError(new ParseError(
                                $"Multiple declarations of predicates with the same name '{predicate.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                predicate.Line,
                                predicate.Start));
                    }
                    predicates.Add(predicate.Name);
                }
            }
        }
        private void CheckForUniquePredicateParameterNames(DomainDecl domain)
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
                            Listener.AddError(new ParseError(
                                $"Multiple declarations of arguments with the same name '{param.Name}' in the predicate '{predicate.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                param.Line,
                                param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private void CheckForUniqueActionNames(DomainDecl domain)
        {
            if (domain.Actions != null)
            {
                List<string> actions = new List<string>();
                foreach (var act in domain.Actions)
                {
                    if (actions.Contains(act.Name))
                    {
                        Listener.AddError(new ParseError(
                            $"Multiple declarations of actions with the same name '{act.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            act.Line,
                            act.Start));
                    }
                    actions.Add(act.Name);
                }
            }
        }
        private void CheckForUniqueActionParameterNames(DomainDecl domain)
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
                            Listener.AddError(new ParseError(
                                    $"Multiple declarations of arguments with the same name '{param.Name}' in the action '{action.Name}'",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Analyser,
                                    param.Line,
                                    param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }
        private void CheckForUniqueAxiomParameterNames(DomainDecl domain)
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
                            Listener.AddError(new ParseError(
                                    $"Multiple declarations of arguments with the same name '{param.Name}' in axiom",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Analyser,
                                    param.Line,
                                    param.Start));
                        }
                        parameterNames.Add(param.Name);
                    }
                }
            }
        }

        private void CheckActionUsesValidPredicates(DomainDecl domain)
        {
            if (domain.Actions != null && domain.Predicates != null)
            {
                foreach (var action in domain.Actions)
                {
                    CheckExpUsesPredicates(action.Preconditions, domain.Predicates.Predicates, domain);
                    CheckExpUsesPredicates(action.Effects, domain.Predicates.Predicates, domain);
                }
            }
        }
        private void CheckAxiomUsesValidPredicates(DomainDecl domain)
        {
            if (domain.Axioms != null && domain.Predicates != null)
            {
                foreach (var axiom in domain.Axioms)
                {
                    CheckExpUsesPredicates(axiom.Context, domain.Predicates.Predicates, domain);
                    CheckExpUsesPredicates(axiom.Implies, domain.Predicates.Predicates, domain);
                }
            }
        }
        private void CheckExpUsesPredicates(IExp node, List<PredicateExp> predicates, DomainDecl domain)
        {
            if (node is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckExpUsesPredicates(child, predicates, domain);
            }
            else if (node is OrExp or)
            {
                CheckExpUsesPredicates(or.Option1, predicates, domain);
                CheckExpUsesPredicates(or.Option2, predicates, domain);
            }
            else if (node is NotExp not)
            {
                CheckExpUsesPredicates(not.Child, predicates, domain);
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
                        Listener.AddError(new ParseError(
                            $"Used predicate '{pred.Name}' did not match the type definitions from the parameters!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            pred.Line,
                            pred.Start));
                    else
                        Listener.AddError(new ParseError(
                            $"Undeclared predicate used '{pred.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            pred.Line,
                            pred.Start));
                }
            }
        }
    }
}
