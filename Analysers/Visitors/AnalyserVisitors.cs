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

        internal void CheckForCorrectPredicateTypes(IWalkable node)
        {
            List<PredicateExp> predicates = new List<PredicateExp>();
            if (Declaration.Domain.Predicates != null)
                predicates.AddRange(Declaration.Domain.Predicates.Predicates);
            if (Declaration.Domain.Functions != null)
                predicates.AddRange(Declaration.Domain.Functions.Functions);

            foreach (var initPred in node)
            {
                if (initPred is PredicateExp pred)
                {
                    var target = predicates.Where(x => x.Name == pred.Name).ToArray()[0];

                    for (int i = 0; i < target.Arguments.Count; i++)
                        if (!pred.Arguments[i].Type.IsTypeOf(target.Arguments[i].Type.Name))
                            Listener.AddError(new ParseError(
                                $"Predicate has an incorrect object type! Expected a '{target.Arguments[i].Type.Name}' but got a '{pred.Arguments[i].Type.Name}'",
                                ParseErrorType.Error,
                                ParseErrorLevel.Analyser,
                                pred.Arguments[i].Line,
                                pred.Arguments[i].Start));
                }
            }
        }

        private void CheckForUndeclaredPredicates(IWalkable node)
        {
            List<PredicateExp> predicates = new List<PredicateExp>();
            if (Declaration.Domain.Predicates != null)
                predicates.AddRange(Declaration.Domain.Predicates.Predicates);
            if (Declaration.Domain.Functions != null)
                predicates.AddRange(Declaration.Domain.Functions.Functions);

            foreach (var item in node)
            {
                if (item is PredicateExp pred && 
                    !predicates.Any(x => x.Name == pred.Name))
                    Listener.AddError(new ParseError(
                        $"Used of undeclared predicate '{pred.Name}'",
                        ParseErrorType.Error,
                        ParseErrorLevel.Analyser,
                        item.Line,
                        item.Start));
            }
        }

        private void CheckForUniqueNames<T>(List<T> nodes, Func<T, ParseError> error) where T : INamedNode
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
                    Listener.AddError(new ParseError(
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
                        Listener.AddError(new ParseError(
                            $"Used of undeclared parameter detected '{node.Name}'",
                            ParseErrorType.Error,
                            ParseErrorLevel.Analyser,
                            node.Line,
                            node.Start));
        }
    }
}
