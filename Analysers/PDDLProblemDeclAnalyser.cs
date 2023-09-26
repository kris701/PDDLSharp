using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers
{
    public class PDDLProblemDeclAnalyser : IAnalyser<ProblemDecl>
    {
        public IErrorListener Listener { get; }

        public PDDLProblemDeclAnalyser(IErrorListener listener)
        {
            Listener = listener;
        }

        public void PostAnalyse(ProblemDecl decl)
        {   
            // Basics
            CheckForBasicProblem(decl);

            // Declare Checking
            CheckDeclaredVsUsedObjects(decl);

            // Unique Name Checking
            CheckForUniqueObjectNames(decl);

            // Validity Checking
            CheckForValidGoal(decl);
        }

        public void PreAnalyse(string text)
        {
            throw new NotImplementedException();
        }

        private void CheckForBasicProblem(ProblemDecl domain)
        {
            if (domain.DomainName == null)
                Listener.AddError(new ParseError(
                    $"Missing domain name reference",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Objects == null)
                Listener.AddError(new ParseError(
                    $"Missing objects declaration",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Objects != null && domain.Objects.Objs.Count == 0)
                Listener.AddError(new ParseError(
                    $"Missing objects",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Init == null)
                Listener.AddError(new ParseError(
                    $"Missing Init declaration",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Init != null && domain.Init.Predicates.Count == 0)
                Listener.AddError(new ParseError(
                    $"No init predicates declared",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
            if (domain.Goal == null)
                Listener.AddError(new ParseError(
                    $"Missing Goal declaration",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line,
                    domain.Start));
        }

        private void CheckDeclaredVsUsedObjects(ProblemDecl problem)
        {
            if (problem.Objects != null)
            {
                foreach (var obj in problem.Objects.Objs)
                {
                    if (problem.FindNames(obj.Name).Count == 1)
                        Listener.AddError(new ParseError(
                            $"Unused object detected '{obj.Name}'",
                            ParseErrorType.Message,
                            ParseErrorLevel.Analyser,
                            obj.Line,
                            obj.Start));
                }
            }
        }

        private void CheckForUniqueObjectNames(ProblemDecl problem)
        {
            if (problem.Objects != null)
            {
                List<string> objs = new List<string>();
                foreach (var obj in problem.Objects.Objs)
                {
                    if (objs.Contains(obj.Name))
                    {
                        Listener.AddError(new ParseError(
                                $"Multiple declarations of object with the same name '{obj.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                obj.Line,
                                obj.Start));
                    }
                    objs.Add(obj.Name);
                }
            }
        }
        
        private void CheckForValidGoal(ProblemDecl problem)
        {
            if (problem.Goal != null)
            {
                if (!DoesAnyPredicatesExist(problem.Goal.GoalExp))
                {
                    Listener.AddError(new ParseError(
                        $"No actual goal predicates in the goal declaration!",
                        ParseErrorType.Warning,
                        ParseErrorLevel.Analyser,
                        problem.Goal.GoalExp.Line,
                        problem.Goal.GoalExp.Start));
                }
            }
        }
        private bool DoesAnyPredicatesExist(IExp exp)
        {
            if (exp is AndExp and)
            {
                foreach (var child in and.Children)
                    if (DoesAnyPredicatesExist(child))
                        return true;
            }
            else if (exp is OrExp or)
            {
                if (DoesAnyPredicatesExist(or.Option1))
                    return true;
                if (DoesAnyPredicatesExist(or.Option2))
                    return true;
            }
            else if (exp is NotExp not)
            {
                return DoesAnyPredicatesExist(not.Child);
            }
            else if (exp is PredicateExp)
            {
                return true;
            }
            return false;
        }
    }
}
