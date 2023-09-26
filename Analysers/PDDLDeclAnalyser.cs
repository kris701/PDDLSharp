using ErrorListeners;
using Models;
using Models.Domain;
using Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLParser.Analysers
{
    public class PDDLDeclAnalyser : IAnalyser<PDDLDecl>
    {
        public void PostAnalyse(PDDLDecl decl, IErrorListener listener)
        {
            IAnalyser<ProblemDecl> problemAnalyser = new PDDLProblemDeclAnalyser();
            problemAnalyser.PostAnalyse(decl.Problem, listener);
            IAnalyser<DomainDecl> domainAnalyser = new PDDLDomainDeclAnalyser();
            domainAnalyser.PostAnalyse(decl.Domain, listener);

            // Declare Checking
            CheckForUndeclaredProblemObjects(decl.Problem, decl.Domain, listener);
            CheckForUndeclaredPreconditionsInInits(decl.Domain, decl.Problem, listener);
            CheckForUndeclaredPreconditionsInGoal(decl.Domain, decl.Problem, listener);

            // Types
            CheckObjectDeclarationTypes(decl.Domain, decl.Problem, listener);
            CheckForInitDeclarationTypes(decl.Domain, decl.Problem, listener);
            CheckForGoalDeclarationTypes(decl.Domain, decl.Problem, listener);
        }

        public void PreAnalyse(string text, IErrorListener listener)
        {
            throw new NotImplementedException();
        }

        private void CheckForUndeclaredProblemObjects(ProblemDecl problem, DomainDecl domain, IErrorListener listener)
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
                                    listener.AddError(new ParseError(
                                        $"Undeclared object detected!",
                                        ParseErrorType.Error,
                                        ParseErrorLevel.Analyser,
                                        ParserErrorCode.UseOfUndeclaredObject,
                                        arg.Line,
                                        arg.Start));
                                }
                            }
                        }
                    }
                }

                if (problem.Goal != null)
                    CheckForUndeclaredExpObjects(problem.Goal.GoalExp, objects, listener);
            }
        }
        private void CheckForUndeclaredExpObjects(IExp exp, List<NameExp> objects, IErrorListener listener)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    CheckForUndeclaredExpObjects(child, objects, listener);
            }
            else if (exp is OrExp or)
            {
                CheckForUndeclaredExpObjects(or.Option1, objects, listener);
                CheckForUndeclaredExpObjects(or.Option2, objects, listener);
            }
            else if (exp is NotExp not)
            {
                CheckForUndeclaredExpObjects(not.Child, objects, listener);
            }
            else if (exp is PredicateExp pred)
            {
                foreach (var arg in pred.Arguments)
                {
                    if (!objects.Any(x => x.Name == arg.Name))
                    {
                        listener.AddError(new ParseError(
                            $"Undeclared object detected!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.UseOfUndeclaredObject,
                            arg.Line,
                            arg.Start));
                    }
                }
            }
        }

        private void CheckForUndeclaredPreconditionsInInits(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Init != null && domain.Predicates != null)
            {
                foreach (var init in problem.Init.Predicates)
                {
                    if (init is PredicateExp pred)
                        if (!domain.Predicates.Predicates.Any(x => x.Name == pred.Name))
                            listener.AddError(new ParseError(
                                $"Used of undeclared predicate in problem '{pred.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                ParserErrorCode.UseOfUndeclaredPredicate,
                                init.Line,
                                init.Start));
                }
            }
        }
        private void CheckForUndeclaredPreconditionsInGoal(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Goal != null && domain.Predicates != null)
                DoesExpContainValidPredicates(problem.Goal.GoalExp, domain.Predicates.Predicates, listener);
        }
        private void DoesExpContainValidPredicates(IExp exp, List<PredicateExp> predicates, IErrorListener listener)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    DoesExpContainValidPredicates(child, predicates, listener);
            }
            else if (exp is OrExp or)
            {
                DoesExpContainValidPredicates(or.Option1, predicates, listener);
                DoesExpContainValidPredicates(or.Option2, predicates, listener);
            }
            else if (exp is NotExp not)
            {
                DoesExpContainValidPredicates(not.Child, predicates, listener);
            }
            else if (exp is PredicateExp pred)
            {
                if (!predicates.Any(x => x.Name == pred.Name))
                    listener.AddError(new ParseError(
                        $"Used of undeclared predicate in expression '{pred.Name}'",
                        ParseErrorType.Error,
                        ParseErrorLevel.Analyser,
                        ParserErrorCode.UseOfUndeclaredPredicate,
                        pred.Line,
                        pred.Start));
            }
        }

        private void CheckObjectDeclarationTypes(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Objects != null)
            {
                foreach(var obj in problem.Objects.Objs)
                    if (!domain.ContainsType(obj.Type.Name))
                        listener.AddError(new ParseError(
                            $"Unknown type for object! '{obj.Type}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            ParserErrorCode.InvalidObjectType,
                            obj.Line,
                            obj.Start));
            }
        }
        private void CheckForInitDeclarationTypes(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
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
                                listener.AddError(new ParseError(
                                    $"Invalid type for init precondition! Got '{pred.Arguments[i].Type.Name}' but expected '{target.Arguments[i].Type.Name}'",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Analyser,
                                    ParserErrorCode.InvalidPredicateType,
                                    init.Line,
                                    init.Start));
                            }
                        }
                    }
                }
            }
        }
        private void CheckForGoalDeclarationTypes(DomainDecl domain, ProblemDecl problem, IErrorListener listener)
        {
            if (problem.Goal != null && domain.Predicates != null)
                CheckExpUsesPredicates(problem.Goal.GoalExp, domain.Predicates.Predicates, listener, domain);
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
