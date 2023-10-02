using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
                decl.Predicates.Add(
                    new PredicateExp(
                        new ASTNode(),
                        decl,
                        "=",
                        new List<NameExp>() {
                            new NameExp(decl, "?l"),
                            new NameExp(decl, "?r")
                        }));
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
            DecorateTypesNamesWithParameterType(node.Parameters, new List<INode>() {
                node.Preconditions,
                node.Effects
            });
        }

        #endregion

        #region DurativeActionDecl

        public void Visit(DurativeActionDecl node)
        {
            DecorateTypesNamesWithParameterType(node.Parameters, new List<INode>() {
                node.Condition,
                node.Effects,
                node.Duration
            });

        }

        #endregion

        #region AxiomDecl

        public void Visit(AxiomDecl node)
        {
            DecorateTypesNamesWithParameterType(node.Vars, new List<INode>() {
                node.Context,
                node.Implies
            });
        }

        #endregion

        #region DerivedDecl

        public void Visit(DerivedDecl node)
        {

        }

        #endregion
    }
}
