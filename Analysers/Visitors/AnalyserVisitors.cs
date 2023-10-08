﻿using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using System.Xml.Linq;

namespace PDDLSharp.Analysers.Visitors
{
    public partial class AnalyserVisitors
    {
        public IErrorListener Listener { get; internal set; }
        public PDDLDecl Declaration { get; internal set; }

        public AnalyserVisitors(IErrorListener listener, PDDLDecl declaration)
        {
            Listener = listener;
            Declaration = declaration;
        }

        private List<PredicateExp> _predicateCache = new List<PredicateExp>();
        private List<PredicateExp> GetPredicateCache()
        {
            if (_predicateCache.Count == 0)
            {
                if (Declaration.Domain.Predicates != null)
                    _predicateCache.AddRange(Declaration.Domain.Predicates.Predicates);
                if (Declaration.Domain.Functions != null)
                    _predicateCache.AddRange(Declaration.Domain.Functions.Functions);
            }
            return _predicateCache;
        }

        public void CheckForCorrectPredicateTypes(INode node, Func<PredicateExp, NameExp, NameExp, PDDLSharpError> error)
        {
            List<PredicateExp> declaredPredicates = GetPredicateCache();
            var nodePredicates = node.FindTypes<PredicateExp>();

            foreach (var pred in nodePredicates)
            {
                var target = declaredPredicates.FirstOrDefault(x => x.Name == pred.Name);
                if (target != null)
                    for (int i = 0; i < target.Arguments.Count; i++)
                        if (!pred.Arguments[i].Type.IsTypeOf(target.Arguments[i].Type.Name))
                            Listener.AddError(error(target, target.Arguments[i], pred.Arguments[i]));
            }
        }

        public void CheckForUndeclaredPredicates(INode node, Func<PredicateExp, PDDLSharpError> error)
        {
            List<PredicateExp> declaredPredicates = GetPredicateCache();
            var nodePredicates = node.FindTypes<PredicateExp>();

            foreach(var pred in nodePredicates)
                if (!declaredPredicates.Any(x => x.Name == pred.Name))
                    Listener.AddError(error(pred));
        }

        public void CheckForUniqueNames(IWalkable node, Func<INamedNode, PDDLSharpError> error)
        {
            var nodeNames = node.FindTypes<INamedNode>();

            foreach (var subNode in node)
                if (subNode is INamedNode named)
                    if (!OnlyOne(nodeNames, named.Name))
                        Listener.AddError(error(named));
        }

        public void CheckForUnusedParameters(ParameterExp parameters, IWalkable checkIn, Func<NameExp, PDDLSharpError> error)
        {
            var allNodes = checkIn.FindTypes<NameExp>();

            foreach (var param in parameters.Values)
                if (!allNodes.Any(x => x.Name == param.Name && param != x))
                    Listener.AddError(error(param));
        }

        public void CheckForUndeclaredParameters(ParameterExp parameters, List<INode> checkIn, Func<NameExp, PDDLSharpError> error)
        {
            List<NameExp> allNodes = new List<NameExp>();
            foreach (var check in checkIn)
                allNodes.AddRange(check.FindTypes<NameExp>(new List<Type>() { typeof(ExistsExp), typeof(ForAllExp) }));
            foreach (var node in allNodes)
                if (node.Name.Contains("?"))
                    if (!parameters.Values.Any(x => x.Name == node.Name))
                        Listener.AddError(error(node));
        }

        public bool OnlyOne<T>(List<T> allItems, string targetName) where T : INamedNode
        {
            int count = 0;
            foreach (var type in allItems)
            {
                if (type.Name == targetName)
                    count++;
                if (count > 1)
                    return false;
            }
            return true;
        }
    }
}
