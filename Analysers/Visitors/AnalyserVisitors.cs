using PDDLSharp.ErrorListeners;
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

        public void CheckForCorrectPredicateTypes(IWalkable node, Func<PredicateExp, NameExp, NameExp, PDDLSharpError> error)
        {
            List<PredicateExp> predicates = GetPredicateCache();

            foreach (var subNode in node)
            {
                if (subNode is PredicateExp pred)
                {
                    var target = predicates.FirstOrDefault(x => x.Name == pred.Name);
                    if (target != null)
                        for (int i = 0; i < target.Arguments.Count; i++)
                            if (!pred.Arguments[i].Type.IsTypeOf(target.Arguments[i].Type.Name))
                                Listener.AddError(error(target, target.Arguments[i], pred.Arguments[i]));
                }
                else if (subNode is IWalkable sub)
                    CheckForCorrectPredicateTypes(sub, error);
            }
        }

        public void CheckForUndeclaredPredicates(IWalkable node, Func<PredicateExp, PDDLSharpError> error)
        {
            List<PredicateExp> predicates = GetPredicateCache();

            foreach (var item in node)
            {
                if (item is PredicateExp pred &&
                    !predicates.Any(x => x.Name == pred.Name))
                    Listener.AddError(error(pred));
                //Listener.AddError(new PDDLSharpError(
                //    $"Used of undeclared predicate '{pred.Name}'",
                //    ParseErrorType.Error,
                //    ParseErrorLevel.Analyser,
                //    item.Line,
                //    item.Start));
            }
        }

        public void CheckForUniqueNames<T>(List<T> nodes, Func<T, PDDLSharpError> error) where T : INamedNode
        {
            List<string> items = new List<string>();
            foreach (var node in nodes)
            {
                if (items.Contains(node.Name))
                    Listener.AddError(error(node));
                items.Add(node.Name);
            }
        }

        public void CheckForUnusedParameters(ParameterExp parameters, IWalkable checkIn, Func<NameExp, PDDLSharpError> error)
        {
            var allNodes = checkIn.FindTypes<NameExp>();
            foreach (var param in parameters.Values)
                if (!allNodes.Any(x => x.Name == param.Name && param != x))
                    Listener.AddError(error(param));
                    //Listener.AddError(new PDDLSharpError(
                    //    $"Unused parameter detected '{param.Name}'",
                    //    ParseErrorType.Warning,
                    //    ParseErrorLevel.Analyser,
                    //    param.Line,
                    //    param.Start));
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
                        //Listener.AddError(new PDDLSharpError(
                        //    $"Used of undeclared parameter detected '{node.Name}'",
                        //    ParseErrorType.Error,
                        //    ParseErrorLevel.Analyser,
                        //    node.Line,
                        //    node.Start));
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
