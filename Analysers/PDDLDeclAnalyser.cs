using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers
{
    public class PDDLDeclAnalyser : IAnalyser<PDDLDecl>
    {
        public IErrorListener Listener { get; }

        public PDDLDeclAnalyser(IErrorListener listener)
        {
            Listener = listener;
        }

        public void PostAnalyse(PDDLDecl decl)
        {
            IAnalyser<ProblemDecl> problemAnalyser = new PDDLProblemDeclAnalyser(Listener);
            problemAnalyser.PostAnalyse(decl.Problem);
            IAnalyser<DomainDecl> domainAnalyser = new PDDLDomainDeclAnalyser(Listener);
            domainAnalyser.PostAnalyse(decl.Domain);

            // Declare Checking
            CheckForUndeclaredProblemObjects(decl.Problem, decl.Domain);
            CheckForUndeclaredPreconditionsInInits(decl.Domain, decl.Problem);
            CheckForUndeclaredPreconditionsInGoal(decl.Domain, decl.Problem);

            // Types
            CheckObjectDeclarationTypes(decl.Domain, decl.Problem);
            CheckForInitDeclarationTypes(decl.Domain, decl.Problem);
            CheckForGoalDeclarationTypes(decl.Domain, decl.Problem);
        }

        public void PreAnalyse(string text)
        {
            throw new NotImplementedException();
        }

        private void CheckForUndeclaredProblemObjects(ProblemDecl problem, DomainDecl domain)
        {
            if (problem.Objects != null)
            {
                List<NameExp> objects = problem.Objects.Objs;
                if (domain.Constants != null)
                    objects.AddRange(domain.Constants.Constants);

                if (problem.Init != null)
                {
                    foreach (var init in problem.Init.Predicates)
                    {
                        if (init is PredicateExp pred)
                        {
                            foreach (var arg in pred.Arguments)
                            {
                                if (!objects.Any(x => x.Name == arg.Name))
                                {
                                    Listener.AddError(new ParseError(
                                        $"Undeclared object detected: '{arg.Name}'",
                                        ParseErrorType.Error,
                                        ParseErrorLevel.Analyser,
                                        arg.Line,
                                        arg.Start));
                                }
                            }
                        }
                    }
                }

                if (problem.Goal != null)
                    CheckForUndeclaredExpObjects(problem.Goal.GoalExp, objects);
            }
        }
        private void CheckForUndeclaredExpObjects(IExp exp, List<NameExp> objects)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForUndeclaredExpObjects(child, objects);
            }
            else if (exp is OrExp or)
            {
                foreach (var option in or.Options)
                    CheckForUndeclaredExpObjects(option, objects);
            }
            else if (exp is NotExp not)
            {
                CheckForUndeclaredExpObjects(not.Child, objects);
            }
            else if (exp is PredicateExp pred)
            {
                foreach (var arg in pred.Arguments)
                {
                    if (!objects.Any(x => x.Name == arg.Name))
                    {
                        Listener.AddError(new ParseError(
                            $"Undeclared object detected: {arg.Name}",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            arg.Line,
                            arg.Start));
                    }
                }
            }
        }

        private void CheckForUndeclaredPreconditionsInInits(DomainDecl domain, ProblemDecl problem)
        {
            if (problem.Init != null && domain.Predicates != null)
            {
                foreach (var init in problem.Init.Predicates)
                {
                    if (init is PredicateExp pred)
                        if (!domain.Predicates.Predicates.Any(x => x.Name == pred.Name))
                            Listener.AddError(new ParseError(
                                $"Used of undeclared predicate in problem '{pred.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                init.Line,
                                init.Start));
                }
            }
        }
        private void CheckForUndeclaredPreconditionsInGoal(DomainDecl domain, ProblemDecl problem)
        {
            if (problem.Goal != null && domain.Predicates != null)
                DoesExpContainValidPredicates(problem.Goal.GoalExp, domain.Predicates.Predicates);
        }
        private void DoesExpContainValidPredicates(IExp exp, List<PredicateExp> predicates)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    DoesExpContainValidPredicates(child, predicates);
            }
            else if (exp is OrExp or)
            {
                foreach (var option in or.Options)
                    DoesExpContainValidPredicates(option, predicates);
            }
            else if (exp is NotExp not)
            {
                DoesExpContainValidPredicates(not.Child, predicates);
            }
            else if (exp is PredicateExp pred)
            {
                if (!predicates.Any(x => x.Name == pred.Name))
                    Listener.AddError(new ParseError(
                        $"Used of undeclared predicate in expression '{pred.Name}'",
                        ParseErrorType.Error,
                        ParseErrorLevel.Analyser,
                        pred.Line,
                        pred.Start));
            }
        }

        private void CheckObjectDeclarationTypes(DomainDecl domain, ProblemDecl problem)
        {
            if (problem.Objects != null)
            {
                foreach(var obj in problem.Objects.Objs)
                    if (!ContainsType(domain, obj.Type.Name))
                        Listener.AddError(new ParseError(
                            $"Unknown type for object! '{obj.Type.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            obj.Line,
                            obj.Start));
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
        private void CheckForInitDeclarationTypes(DomainDecl domain, ProblemDecl problem)
        {
            if (problem.Init != null && domain.Predicates != null)
            {
                foreach(var init in problem.Init.Predicates)
                {
                    if (init is PredicateExp pred)
                    {
                        var target = domain.Predicates.Predicates.Single(x => x.Name == pred.Name);
                        for (int i = 0; i < pred.Arguments.Count; i++)
                        {
                            if (!pred.Arguments[i].Type.IsTypeOf(target.Arguments[i].Type.Name))
                            {
                                if (domain.Constants != null)
                                {
                                    if (domain.Constants.Constants.Any(x => x.Name == pred.Arguments[i].Name))
                                        continue;
                                }
                                Listener.AddError(new ParseError(
                                    $"Invalid type for init precondition! Got '{pred.Arguments[i].Type.Name}' but expected '{target.Arguments[i].Type.Name}'",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Analyser,
                                    init.Line,
                                    init.Start));
                            }
                        }
                    }
                }
            }
        }
        private void CheckForGoalDeclarationTypes(DomainDecl domain, ProblemDecl problem)
        {
            if (problem.Goal != null && domain.Predicates != null)
                CheckExpUsesPredicates(problem.Goal.GoalExp, domain.Predicates.Predicates, domain);
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
                foreach (var option in or.Options)
                    CheckExpUsesPredicates(option, predicates, domain);
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
