﻿using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.PDDL.Problem;

namespace PDDLSharp.Contextualisers.Visitors
{
    public partial class ContextualiserVisitors
    {
        public void Visit(ProblemDecl decl)
        {
            foreach (var node in decl)
                Visit((dynamic)node);
        }

        #region ProblemNameDecl

        public void Visit(ProblemNameDecl node)
        {

        }

        #endregion

        #region DomainNameRefDecl

        public void Visit(DomainNameRefDecl node)
        {

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
            DecorateObjects(node);
        }

        private void DecorateObjects(ObjectsDecl decl)
        {
            var allPredicates = Declaration.Problem.FindTypes<PredicateExp>();
            foreach (var predicate in allPredicates)
            {
                for (int i = 0; i < predicate.Arguments.Count; i++)
                {
                    var obj = decl.Objs.FirstOrDefault(x => x.Name == predicate.Arguments[i].Name);
                    if (obj != null)
                        predicate.Arguments[i].Type.Name = obj.Type.Name;
                }
            }
        }

        #endregion

        #region InitsDecl

        public void Visit(InitDecl node)
        {

        }

        #endregion

        #region GoalDecl

        public void Visit(GoalDecl node)
        {
            var allParametized = node.FindTypes<IParametized>();
            foreach (var parametized in allParametized)
                DecorateTypesNamesWithParameterType(parametized);
        }

        #endregion

        #region MetricDecl

        public void Visit(MetricDecl node)
        {

        }

        #endregion
    }
}
