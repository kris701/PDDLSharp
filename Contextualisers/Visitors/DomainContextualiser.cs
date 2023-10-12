using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using System;

namespace PDDLSharp.Contextualisers.Visitors
{
    public partial class ContextualiserVisitors
    {
        public void Visit(DomainDecl decl)
        {
            foreach (var node in decl)
                Visit((dynamic)node);
        }

        #region DomainNameDecl

        public void Visit(DomainNameDecl node)
        {

        }

        #endregion

        #region RequirementsDecl

        public void Visit(RequirementsDecl node)
        {

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

        }

        #endregion

        #region TypesDecl

        public void Visit(TypesDecl node)
        {

        }

        #endregion

        #region ConstantsDecl

        public void Visit(ConstantsDecl node)
        {
            DecorateFromConstants(node);
        }

        private void DecorateFromConstants(ConstantsDecl decl)
        {
            var allOfConstant = Declaration.Problem.FindTypes<NameExp>();
            foreach (var instance in allOfConstant)
            {
                var target = decl.Constants.FirstOrDefault(x => x.Name == instance.Name);
                if (target != null)
                    instance.Type.Name = target.Type.Name;
            }
        }

        #endregion

        #region PredicatesDecl

        public void Visit(PredicatesDecl node)
        {
            InsertDefaultPredicates(node);
        }

        private void InsertDefaultPredicates(PredicatesDecl decl)
        {
            if (!decl.Predicates.Any(x => x.Name == "="))
            {
                var equals = new PredicateExp(
                        new ASTNode(),
                        decl,
                        "=",
                        new List<NameExp>() {
                            new NameExp(decl, "?l"),
                            new NameExp(decl, "?r")
                        });
                equals.IsHidden = true;
                decl.Predicates.Add(equals);
            }
        }

        #endregion

        #region FunctionsDecl

        public void Visit(FunctionsDecl node)
        {

        }

        #endregion

        #region ActionDecl

        public void Visit(ActionDecl node)
        {
            DecorateTypesNamesWithParameterType(node);
        }

        #endregion

        #region DurativeActionDecl

        public void Visit(DurativeActionDecl node)
        {
            DecorateTypesNamesWithParameterType(node);

        }

        #endregion

        #region AxiomDecl

        public void Visit(AxiomDecl node)
        {
            DecorateTypesNamesWithParameterType(node);
        }

        #endregion

        #region DerivedDecl

        public void Visit(DerivedDecl node)
        {
            var targets = Declaration.Domain.FindNames(node.Predicate.Name);
            targets.AddRange(Declaration.Problem.FindNames(node.Predicate.Name));

            for(int i = 0; i < targets.Count; i++)
            {
                if (targets[i] is DerivedPredicateExp derivedPred)
                {
                    derivedPred.AddDecl(node);
                }
                else if (targets[i] is PredicateExp pred && pred.Parent is not DerivedDecl)
                {
                    if (pred.Arguments.Count != node.Predicate.Arguments.Count)
                        Listener.AddError(new PDDLSharpError(
                            $"Derived declaration expected {node.Predicate.Arguments.Count} but here {pred.Arguments.Count} is given!",
                            ParseErrorType.Error,
                            ParseErrorLevel.Contexturaliser,
                            pred.Line,
                            pred.Start
                                ));

                    var copy = pred.Copy(pred.Parent);
                    var newNode = new DerivedPredicateExp(copy.Parent, copy.Name, copy.Arguments, new List<DerivedDecl>() { node });
                    if (copy.Parent is IWalkable walk)
                        walk.Replace(pred, newNode);
                }
            }
        }

        #endregion
    }
}
