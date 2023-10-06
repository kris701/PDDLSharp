using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Plans;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.Simulators.StateSpace
{
    public class StateSpaceSimulator : IStateSpaceSimulator
    {
        public PDDLDecl Declaration { get; internal set; }
        public HashSet<GroundedPredicate> State { get; internal set; }
        public int Cost { get; internal set; } = 0;

        private HashSet<GroundedPredicate> _tempAdd = new HashSet<GroundedPredicate>();
        private HashSet<GroundedPredicate> _tempDel = new HashSet<GroundedPredicate>();

        public StateSpaceSimulator(PDDLDecl declaration)
        {
            Declaration = declaration;
            if (!Declaration.IsContextualised)
            {
                IErrorListener listener = new ErrorListener(ParseErrorType.Error);
                IContextualiser contextualiser = new PDDLContextualiser(listener);
                contextualiser.Contexturalise(Declaration);
            }

            State = new HashSet<GroundedPredicate>();
            State = GenerateInitialState();
        }

        private HashSet<GroundedPredicate> GenerateInitialState()
        {
            var state = new HashSet<GroundedPredicate>();
            if (Declaration.Problem.Init != null)
                foreach (var item in Declaration.Problem.Init.Predicates)
                    if (item is PredicateExp predicate)
                        state.Add(new GroundedPredicate(predicate));
            return state;
        }

        public bool Contains(GroundedPredicate op) => State.Contains(op);

        public bool Contains(string op, params string[] arguments) => Contains(new GroundedPredicate(op, GetNameExpFromString(arguments)));

        public void Reset()
        {
            Cost = 0;
            State = new HashSet<GroundedPredicate>();
            State = GenerateInitialState();
        }

        public void Step(string actionName, params string[] arguments)
        {
            if (Declaration.Problem.Objects == null)
                throw new ArgumentException("Objects not declared in problem");
            Step(actionName, GetNameExpFromString(arguments));
        }

        private List<NameExp> GetNameExpFromString(string[] arguments)
        {
            var args = new List<NameExp>();
            foreach (var arg in arguments)
            {
                NameExp? obj = null;
                if (Declaration.Problem.Objects != null)
                    obj = Declaration.Problem.Objects.Objs.FirstOrDefault(x => x.Name == arg.ToLower());
                if (obj == null && Declaration.Domain.Constants != null)
                    obj = Declaration.Domain.Constants.Constants.FirstOrDefault(x => x.Name == arg.ToLower());

                if (obj == null)
                    throw new ArgumentException($"Cannot find object (or constant) '{arg}'");
                args.Add(new NameExp(obj));
            }
            return args;
        }

        public void Step(string actionName) => Step(actionName, new List<NameExp>());

        private void Step(string actionName, List<NameExp> arguments)
        {
            actionName = actionName.ToLower();

            var targetAction = GetTargetAction(actionName);

            if (targetAction.Parameters.Values.Count != arguments.Count)
                throw new ArgumentOutOfRangeException($"Action takes '{targetAction.Parameters.Values.Count}' arguments, but was given '{arguments.Count}'");

            var argDict = GenerateArgDict(targetAction, arguments);
            if (!IsAllPredicatesTrue(targetAction.Preconditions, argDict, false))
                throw new ArgumentException("Not all precondition predicates are set!");

            _tempAdd.Clear();
            _tempDel.Clear();
            ExecuteEffect(targetAction.Effects, argDict, false);
            foreach (var item in _tempAdd)
                State.Add(item);
            foreach (var item in _tempDel)
                State.Remove(item);

            Cost++;
        }

        private ActionDecl GetTargetAction(string actionName)
        {
            var targetAction = Declaration.Domain.Actions.FirstOrDefault(x => x.Name == actionName);
            if (targetAction == null)
                throw new ArgumentNullException($"Could not find an action called '{actionName}'");
            return targetAction;
        }

        private Dictionary<string, NameExp> GenerateArgDict(ActionDecl targetAction, List<NameExp> arguments)
        {
            var argDict = new Dictionary<string, NameExp>();
            for (int i = 0; i < arguments.Count; i++)
                argDict.Add(targetAction.Parameters.Values[i].Name, new NameExp(arguments[i]));
            return argDict;
        }

        private void ExecuteEffect(INode node, Dictionary<string, NameExp> dict, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                var op = new GroundedPredicate(predicate, GroundArguments(predicate.Arguments, dict));
                if (isNegative)
                    _tempDel.Add(op);
                else
                    _tempAdd.Add(op);
            } 
            else if (node is NotExp not)
            {
                ExecuteEffect(not.Child, dict, !isNegative);
            } 
            else if (node is WhenExp when)
            {
                if (IsAllPredicatesTrue(when.Condition, dict, false))
                    ExecuteEffect(when.Effect, dict, false);
            }
            else if (node is ForAllExp all)
            {
                var permutations = GenerateParameterPermutations(all.Parameters.Values, dict, 0);
                foreach (var permutation in permutations)
                    ExecuteEffect(all.Expression, permutation, isNegative);
            }
            else if (node is IWalkable walk)
            {
                foreach (var subNode in walk)
                    ExecuteEffect(subNode, dict, isNegative);
            }
        }

        private List<Dictionary<string, NameExp>> GenerateParameterPermutations(List<NameExp> values, Dictionary<string, NameExp> source, int index)
        {
            List<Dictionary<string, NameExp>> returnList = new List<Dictionary<string, NameExp>>();

            if (index >= values.Count)
            {
                returnList.Add(CopyDict(source));
                return returnList;
            }

            if (Declaration.Problem.Objects != null)
            {
                var allOfType = Declaration.Problem.Objects.Objs.Where(x => x.Type.IsTypeOf(values[index].Type.Name));
                foreach(var ofType in allOfType)
                {
                    var newDict = CopyDict(source);
                    if (newDict.ContainsKey(values[index].Name))
                        newDict[values[index].Name] = new NameExp(ofType);
                    else
                        newDict.Add(values[index].Name, new NameExp(ofType));

                    returnList.AddRange(GenerateParameterPermutations(values, newDict, index + 1));
                }
            }

            return returnList;
        }

        private bool IsAllPredicatesTrue(INode node, Dictionary<string, NameExp> dict, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                var op = new GroundedPredicate(predicate, GroundArguments(predicate.Arguments, dict));
                if (isNegative)
                    return !State.Contains(op);
                else
                    return State.Contains(op);
            }
            else if (node is NotExp not)
            {
                return IsAllPredicatesTrue(not.Child, dict, !isNegative);
            }
            else if (node is OrExp or)
            {
                foreach (var subNode in or)
                    if (IsAllPredicatesTrue(subNode, dict, isNegative))
                        return true;
            }
            else if (node is IWalkable walk)
            {
                foreach(var subNode in walk)
                    if (!IsAllPredicatesTrue(subNode, dict, isNegative))
                        return false;
            }
            return true;
        }

        private List<NameExp> GroundArguments(List<NameExp> args, Dictionary<string, NameExp> dict)
        {
            List<NameExp> newObjects = new List<NameExp>();

            foreach (var arg in args)
            {
                if (!arg.Type.IsTypeOf(dict[arg.Name].Type.Name))
                    throw new ArgumentException("Argument types did not match!");
                newObjects.Add(dict[arg.Name]);
            }

            return newObjects;
        }

        private Dictionary<string, NameExp> CopyDict(Dictionary<string, NameExp> from)
        {
            Dictionary<string, NameExp> newDict = new Dictionary<string, NameExp>();

            foreach (var key in from.Keys)
                newDict.Add(key, new NameExp(from[key]));

            return newDict;
        }
    }
}
