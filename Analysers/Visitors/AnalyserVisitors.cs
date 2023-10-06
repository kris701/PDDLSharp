using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        internal void CheckForCorrectPredicateTypes(IWalkable node)
        {
            List<PredicateExp> predicates = GetPredicateCache();

            foreach (var initPred in node)
            {
                if (initPred is PredicateExp pred)
                {
                    var target = predicates.FirstOrDefault(x => x.Name == pred.Name);
                    if (target != null)
                    {
                        for (int i = 0; i < target.Arguments.Count; i++)
                            if (!pred.Arguments[i].Type.IsTypeOf(target.Arguments[i].Type.Name))
                                Listener.AddError(new PDDLSharpError(
                                    $"Predicate has an incorrect object type! Expected a '{target.Arguments[i].Type.Name}' but got a '{pred.Arguments[i].Type.Name}'",
                                    ParseErrorType.Error,
                                    ParseErrorLevel.Analyser,
                                    pred.Arguments[i].Line,
                                    pred.Arguments[i].Start));
                    }
                }
            }
        }

        private void CheckForUndeclaredPredicates(IWalkable node)
        {
            List<PredicateExp> predicates = GetPredicateCache();

            foreach (var item in node)
            {
                if (item is PredicateExp pred && 
                    !predicates.Any(x => x.Name == pred.Name))
                    Listener.AddError(new PDDLSharpError(
                        $"Used of undeclared predicate '{pred.Name}'",
                        ParseErrorType.Error,
                        ParseErrorLevel.Analyser,
                        item.Line,
                        item.Start));
            }
        }

        private void CheckForUniqueNames<T>(List<T> nodes, Func<T, PDDLSharpError> error) where T : INamedNode
        {
            List<string> items = new List<string>();
            foreach (var node in nodes)
            {
                if (items.Contains(node.Name))
                    Listener.AddError(error(node));
                items.Add(node.Name);
            }
        }

        private void CheckForUnusedParameters(ParameterExp parameters, IWalkable checkIn)
        {
            var allNodes = checkIn.FindTypes<NameExp>();
            foreach(var param in parameters.Values)
                if (!allNodes.Any(x => x.Name == param.Name && param != x))
                    Listener.AddError(new PDDLSharpError(
                        $"Unused parameter detected '{param.Name}'",
                        ParseErrorType.Warning,
                        ParseErrorLevel.Analyser,
                        param.Line,
                        param.Start));
        }

        private void CheckForUndeclaredParameters(ParameterExp parameters, List<INode> checkIn)
        {
            List<NameExp> allNodes = new List<NameExp>();
            foreach (var check in checkIn)
                allNodes.AddRange(check.FindTypes<NameExp>(new List<Type>() { typeof(ExistsExp), typeof(ForAllExp) }));
            foreach (var node in allNodes)
                if (node.Name.Contains("?"))
                    if (!parameters.Values.Any(x => x.Name == node.Name))
                        Listener.AddError(new PDDLSharpError(
                            $"Used of undeclared parameter detected '{node.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            node.Line,
                            node.Start));
        }

        private bool OnlyOne<T>(List<T> allItems, string targetName) where T : INamedNode
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
