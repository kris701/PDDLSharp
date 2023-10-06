using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Analysers.Visitors
{
    public partial class AnalyserVisitors
    {
        public void Visit(ProblemDecl node)
        {
            // Basic
            CheckForBasicProblem(node);

            // Analyse individual components
            foreach (var item in node)
                Visit((dynamic)item);
        }

        private void CheckForBasicProblem(ProblemDecl problem)
        {
            if (problem.DomainName == null)
                Listener.AddError(new ParseError(
                    $"Missing domain name reference",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    problem.Line,
                    problem.Start));
            if (problem.Objects == null)
                Listener.AddError(new ParseError(
                    $"Missing objects declaration",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    problem.Line,
                    problem.Start));
            if (problem.Objects != null && problem.Objects.Objs.Count == 0)
                Listener.AddError(new ParseError(
                    $"Missing objects",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    problem.Line,
                    problem.Start));
            if (problem.Init == null)
                Listener.AddError(new ParseError(
                    $"Missing Init declaration",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    problem.Line,
                    problem.Start));
            if (problem.Init != null && problem.Init.Predicates.Count == 0)
                Listener.AddError(new ParseError(
                    $"No init predicates declared",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    problem.Line,
                    problem.Start));
            if (problem.Goal == null)
                Listener.AddError(new ParseError(
                    $"Missing Goal declaration",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    problem.Line,
                    problem.Start));
        }

        #region ProblemNameDecl

        public void Visit(ProblemNameDecl node)
        {

        }

        #endregion

        #region DomainNameRefDecl

        public void Visit(DomainNameRefDecl node)
        {
            if (Declaration.Domain.Name != null &&
                node.Name != Declaration.Domain.Name.Name)
                    Listener.AddError(new ParseError(
                        $"Domain name referenced in problem file ('{node.Name}') does not match the actual domain name ('{Declaration.Domain.Name.Name}')!",
                        ParseErrorType.Warning,
                        ParseErrorLevel.Analyser,
                        node.Line,
                        node.Start));
        }

        #endregion

        #region SituationDecl

        public void Visit(SituationDecl node)
        {

        }

        #endregion

        #region ObjectsDecl
        public void Visit(ObjectsDecl node)
        {
            if (node.Objs.Count == 0)
                Listener.AddError(new ParseError(
                    $"No objects declared",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));

            CheckForUniqueNames(
                node.Objs, 
                (node) => new ParseError(
                    $"An object have been declared multiple times: '{node.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));
            CheckForUndeclaredObjects(node);
            CheckForUnusedObjects(node);
            CheckObjectDeclarationTypes(node);
        }

        private void CheckForUndeclaredObjects(ObjectsDecl node)
        {
            var allNames = new List<NameExp>();
            allNames.AddRange(node.Objs);
            if (Declaration.Domain.Constants != null)
                allNames.AddRange(Declaration.Domain.Constants.Constants);

            foreach (var name in Declaration.Problem.FindTypes<NameExp>(new List<Type>() { typeof(ExistsExp), typeof(ForAllExp) }))
                if (!allNames.Any(x => x.Name == name.Name))
                    if (name.Parent != node)
                        Listener.AddError(new ParseError(
                            $"Undeclared object detected '{name.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            name.Line,
                            name.Start));
        }

        private void CheckForUnusedObjects(ObjectsDecl node)
        {
            var allNames = Declaration.Problem.FindTypes<NameExp>();
            foreach (var obj in node.Objs)
            {
                if (allNames.Count(x => x.Name == obj.Name) == 1)
                {
                    Listener.AddError(new ParseError(
                        $"Unused object detected '{obj.Name}'",
                        ParseErrorType.Message,
                        ParseErrorLevel.Analyser,
                        obj.Line,
                        obj.Start));
                }
            }
        }
        private void CheckObjectDeclarationTypes(ObjectsDecl node)
        {
            if (Declaration.Domain.Types != null)
            {
                foreach (var obj in node.Objs)
                    if (!Declaration.Domain.Types.Types.Any(x => x.Name == obj.Type.Name))
                        Listener.AddError(new ParseError(
                            $"Unknown type for object! '{obj.Type.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            obj.Line,
                            obj.Start));
            }
            else
            {
                foreach (var obj in node.Objs)
                    if (obj.Type.Name != "" && obj.Type.Name != "object")
                        Listener.AddError(new ParseError(
                            $"Unknown type for object! '{obj.Type.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            obj.Line,
                            obj.Start));
            }
        }
        #endregion

        #region InitsDecl

        public void Visit(InitDecl node)
        {
            if (node.Predicates.Count == 0)
                Listener.AddError(new ParseError(
                    $"No init predicates declared",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));

            CheckForUndeclaredPredicates(node);
            CheckForCorrectPredicateTypes(node);
        }

        #endregion

        #region GoalDecl

        public void Visit(GoalDecl node)
        {
            if (node.GoalExp == null)
                Listener.AddError(new ParseError(
                    $"No goal declared",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    node.Line,
                    node.Start));

            CheckForUndeclaredPredicates(node);
            CheckForCorrectPredicateTypes(node);
        }

        #endregion

        #region MetricDecl

        public void Visit(MetricDecl node)
        {
            IsMetricusingOnlyNumericFluents(node);
            CheckForUndeclaredPredicates(node);
        }

        private void IsMetricusingOnlyNumericFluents(MetricDecl node)
        {
            var allNodes = node.MetricExp.FindTypes<INode>();
            foreach(var exp in allNodes)
            {
                if (exp is not PredicateExp && exp is not NumericExp)
                    Listener.AddError(new ParseError(
                        $"The metric expression must only contain predicates or numeric fluents! Currently contains a `{exp.GetType().Name}` node",
                        ParseErrorType.Error,
                        ParseErrorLevel.Analyser,
                        exp.Line,
                        exp.Start));
            }
        }

        #endregion
    }
}
