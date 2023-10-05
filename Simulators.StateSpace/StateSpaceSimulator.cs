using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
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
        public HashSet<Operator> State { get; internal set; }
        public int Cost { get; internal set; } = 0;

        public StateSpaceSimulator(PDDLDecl declaration)
        {
            Declaration = declaration;
            if (!Declaration.IsContextualised)
            {
                IErrorListener listener = new ErrorListener(ParseErrorType.Error);
                IContextualiser contextualiser = new PDDLContextualiser(listener);
                contextualiser.Contexturalise(Declaration);
            }

            State = new HashSet<Operator>();
            if (declaration.Problem.Init != null)
                State = GenerateState(declaration.Problem.Init);
        }

        private HashSet<Operator> GenerateState(InitDecl decl)
        {
            var state = new HashSet<Operator>();
            foreach (var item in decl)
            {
                if (item is PredicateExp predicate)
                    state.Add(new Operator(predicate));
            }
            return state;
        }

        public void Reset()
        {
            Cost = 0;
            State = new HashSet<Operator>();
            if (Declaration.Problem.Init != null)
                State = GenerateState(Declaration.Problem.Init);
        }

        public void Step(string actionName, string[] arguments)
        {
            if (Declaration.Problem.Objects == null)
                throw new ArgumentException("Objects not declared in problem");
            var args = new List<OperatorObject>();
            foreach (var arg in arguments) 
            {
                var obj = Declaration.Problem.Objects.Objs.First(x => x.Name == arg);
                args.Add(new OperatorObject(obj));
            }
            Step(actionName, args);
        }

        public void Step(string actionName) => Step(actionName, new List<OperatorObject>());

        private void Step(string actionName, List<OperatorObject> arguments)
        {
            actionName = actionName.ToLower();
            foreach (var arg in arguments)
            {
                arg.Name = arg.Name.ToLower();
                arg.Type = arg.Type.ToLower();
            }

            var targetAction = GetTargetAction(actionName);

            if (targetAction.Parameters.Values.Count != arguments.Count)
                throw new ArgumentOutOfRangeException($"Action takes '{targetAction.Parameters.Values.Count}' arguments, but was given '{arguments.Count}'");

            CheckifObjectsAreValid(arguments);

            var argDict = GenerateArgDict(targetAction, arguments);
            if (!IsAllPredicatesTrue(targetAction.Preconditions, argDict, false))
                throw new ArgumentException("Not all precondition predicates are set!");

            ExecuteEffect(targetAction.Effects, argDict, false);
        }

        private ActionDecl GetTargetAction(string actionName)
        {
            var targetAction = Declaration.Domain.Actions.FirstOrDefault(x => x.Name == actionName);
            if (targetAction == null)
                throw new ArgumentNullException($"Could not find an action called '{actionName}'");
            return targetAction;
        }

        private void CheckifObjectsAreValid(List<OperatorObject> arguments)
        {
            if (Declaration.Problem.Objects != null)
            {
                foreach (var obj in arguments)
                    if (!Declaration.Problem.Objects.Objs.Any(x => x.Name == obj.Name))
                        throw new ArgumentException($"No object of name '{obj.Name}' in problem!");
            }
        }

        private Dictionary<string, OperatorObject> GenerateArgDict(ActionDecl targetAction, List<OperatorObject> arguments)
        {
            var argDict = new Dictionary<string, OperatorObject>();
            for (int i = 0; i < arguments.Count; i++)
                argDict.Add(targetAction.Parameters.Values[i].Name, new OperatorObject(arguments[i].Name, arguments[i].Type));
            return argDict;
        }

        private void ExecuteEffect(INode node, Dictionary<string, OperatorObject> dict, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                var op = new Operator(predicate.Name, GroundArguments(predicate.Arguments, dict));
                if (isNegative)
                    State.Remove(op);
                else
                    State.Add(op);
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

        private List<Dictionary<string, OperatorObject>> GenerateParameterPermutations(List<NameExp> values, Dictionary<string, OperatorObject> source, int index)
        {
            List<Dictionary<string, OperatorObject>> returnList = new List<Dictionary<string, OperatorObject>>();

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
                        newDict[values[index].Name] = new OperatorObject(ofType.Name);
                    else
                        newDict.Add(values[index].Name, new OperatorObject(ofType.Name));

                    returnList.AddRange(GenerateParameterPermutations(values, newDict, index + 1));
                }
            }

            return returnList;
        }

        private bool IsAllPredicatesTrue(INode node, Dictionary<string, OperatorObject> dict, bool isNegative)
        {
            if (node is PredicateExp predicate)
            {
                var op = new Operator(predicate.Name, GroundArguments(predicate.Arguments, dict));
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
            else if (node is AndExp and)
            {
                foreach(var subNode in and)
                    if (!IsAllPredicatesTrue(subNode, dict, isNegative))
                        return false;
            }
            return true;
        }

        private List<OperatorObject> GroundArguments(List<NameExp> args, Dictionary<string, OperatorObject> dict)
        {
            List<OperatorObject> newObjects = new List<OperatorObject>();

            foreach (var arg in args)
            {
                if (!arg.Type.IsTypeOf(dict[arg.Name].Type))
                    throw new ArgumentException("Argument types did not match!");
                newObjects.Add(new OperatorObject(dict[arg.Name]));
            }

            return newObjects;
        }

        private Dictionary<string,OperatorObject> CopyDict(Dictionary<string, OperatorObject> from)
        {
            Dictionary<string, OperatorObject> newDict = new Dictionary<string, OperatorObject>();

            foreach (var key in from.Keys)
                newDict.Add(key, new OperatorObject(from[key]));

            return newDict;
        }
    }
}
