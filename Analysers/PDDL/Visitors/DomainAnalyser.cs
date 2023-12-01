using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Shared;

namespace PDDLSharp.Analysers.Visitors
{
    public partial class AnalyserVisitors
    {
        public void Visit(DomainDecl node)
        {
            // Basic
            CheckForBasicDomain(node);

            // Analyse individual components
            foreach (var item in node)
                Visit((dynamic)item);
        }

        private void CheckForBasicDomain(DomainDecl domain)
        {
            if (domain.Predicates == null)
                Listener.AddError(new PDDLSharpError(
                    $"Missing predicates declaration.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line));
            if (domain.Predicates != null && domain.Predicates.Predicates.Count == 0)
                Listener.AddError(new PDDLSharpError(
                    $"No predicates defined.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line));
            if (domain.Actions == null)
                Listener.AddError(new PDDLSharpError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line));
            if (domain.Actions != null && domain.Actions.Count == 0)
                Listener.AddError(new PDDLSharpError(
                    $"Missing actions.",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    domain.Line));
        }

        #region DomainNameDecl

        public void Visit(DomainNameDecl node)
        {

        }

        #endregion

        #region RequirementsDecl

        public void Visit(RequirementsDecl node)
        {
            CheckForUniqueNames(
                node,
                (node) => new PDDLSharpError(
                    $"A requirement have been declared multiple times: '{node.Name}'",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    node.Line));
        }

        #endregion

        #region ExtendsDecl

        public void Visit(ExtendsDecl node)
        {

        }

        #endregion

        #region TimelessDecl

        public void Visit(TimelessDecl node)
        {
            CheckForUniqueNames(
                node,
                (node) => new PDDLSharpError(
                    $"A Timeless predicate have been declared multiple times: '{node.Name}'",
                    ParseErrorType.Message,
                    ParseErrorLevel.Analyser,
                    node.Line));
        }

        #endregion

        #region TypesDecl

        public void Visit(TypesDecl node)
        {
            CheckForUniqueNames(
                node,
                (node) => new PDDLSharpError(
                    $"A type have been declared multiple times: '{node.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
            CheckForUnusedTypes(node);
        }

        private void CheckForUnusedTypes(TypesDecl node)
        {
            var allTypes = Declaration.Domain.FindTypes<TypeExp>();
            allTypes.AddRange(Declaration.Problem.FindTypes<TypeExp>());

            foreach (var type in node.Types)
            {
                if (OnlyOne(allTypes, type.Name))
                    Listener.AddError(new PDDLSharpError(
                        $"Unused type detected '{type.Name}'",
                        ParseErrorType.Message,
                        ParseErrorLevel.Analyser,
                        type.Line));
            }
        }

        #endregion

        #region ConstantsDecl

        public void Visit(ConstantsDecl node)
        {
            CheckForUniqueNames(
                node,
                (node) => new PDDLSharpError(
                    $"A constant have been declared multiple times: '{node.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
        }

        #endregion

        #region PredicatesDecl

        public void Visit(PredicatesDecl node)
        {
            CheckForUniqueNames(
                node,
                (node) => new PDDLSharpError(
                    $"A predicate have been declared multiple times: '{node.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
            CheckForUnusedPredicatesInPredicateDecl(node);
        }

        private void CheckForUnusedPredicatesInPredicateDecl(PredicatesDecl node)
        {
            var allPredicates = Declaration.Domain.FindTypes<PredicateExp>();
            allPredicates.AddRange(Declaration.Problem.FindTypes<PredicateExp>());
            foreach (var predicate in node.Predicates)
            {
                if (!predicate.IsHidden && OnlyOne(allPredicates, predicate.Name))
                {
                    Listener.AddError(new PDDLSharpError(
                        $"Unused predicate detected '{predicate.Name}'",
                        ParseErrorType.Message,
                        ParseErrorLevel.Analyser,
                        predicate.Line));
                }
            }
        }

        #endregion

        #region FunctionsDecl

        public void Visit(FunctionsDecl node)
        {
            CheckForUniqueNames(
                node,
                (node) => new PDDLSharpError(
                    $"A function have been declared multiple times: '{node.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
        }

        #endregion

        #region ActionDecl

        public void Visit(ActionDecl node)
        {

            CheckForUndeclaredParameters(
                node,
                (param) => new PDDLSharpError(
                    $"Action '{node.Name}' contains undeclared parameter '{param.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
            CheckForUnusedParameters(
                node,
                (param) => new PDDLSharpError(
                    $"Action '{node.Name}' contains unused parameter '{param.Name}'",
                    ParseErrorType.Warning,
                    ParseErrorLevel.Analyser,
                    param.Line));
            CheckForCorrectPredicateTypes(
                node,
                (pred, expected, actual) => new PDDLSharpError(
                    $"Action '{node.Name}' contains predicate '{pred.Name}' with parameter '{expected.Name}' that expected a type '{expected.Type.Name}' but got a '{actual.Type.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
        }

        #endregion

        #region DurativeActionDecl

        public void Visit(DurativeActionDecl node)
        {
            CheckForUndeclaredParameters(
                node,
                (param) => new PDDLSharpError(
                    $"Durative action '{node.Name}' contains undeclared parameter '{param.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
            CheckForUnusedParameters(
                node,
                (param) => new PDDLSharpError(
                    $"Durative action '{node.Name}' contains unused parameter '{param.Name}'",
                    ParseErrorType.Warning,
                    ParseErrorLevel.Analyser,
                    param.Line));
            CheckForCorrectPredicateTypes(
                node,
                (pred, expected, actual) => new PDDLSharpError(
                    $"Durative action '{node.Name}' contains predicate '{pred.Name}' with parameter '{expected.Name}' that expected a type '{expected.Type.Name}' but got a '{actual.Type.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
        }

        #endregion

        #region AxiomDecl

        public void Visit(AxiomDecl node)
        {
            CheckForUndeclaredParameters(
                node,
                (param) => new PDDLSharpError(
                    $"Axiom contains undeclared parameter '{param.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
            CheckForUnusedParameters(
                node,
                (param) => new PDDLSharpError(
                    $"Axiom contains unused parameter '{param.Name}'",
                    ParseErrorType.Warning,
                    ParseErrorLevel.Analyser,
                    param.Line));
            CheckForCorrectPredicateTypes(
                node,
                (pred, expected, actual) => new PDDLSharpError(
                    $"Axiom contains predicate '{pred.Name}' with parameter '{expected.Name}' that expected a type '{expected.Type.Name}' but got a '{actual.Type.Name}'",
                    ParseErrorType.Error,
                    ParseErrorLevel.Analyser,
                    node.Line));
        }

        #endregion

        #region DerivedDecl

        public void Visit(DerivedDecl node)
        {

        }

        #endregion
    }
}
