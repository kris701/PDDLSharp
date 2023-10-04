using PDDLSharp.Models;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.Problem;
using System;
using System.Collections.Generic;
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
            var args = new List<OperatorObject>();
            foreach (var arg in arguments)
                args.Add(new OperatorObject(arg));
            Step(actionName, args);
        }

        public void Step(string actionName, List<OperatorObject> arguments)
        {
            var targetAction = Declaration.Domain.Actions.FirstOrDefault(x => x.Name == actionName);
            if (targetAction == null)
                throw new ArgumentNullException($"Could not find an action called '{actionName}'");
            if (targetAction.Parameters.Values.Count != arguments.Count)
                throw new ArgumentOutOfRangeException($"Action takes '{targetAction.Parameters.Values.Count}' arguments, but was given '{arguments.Count}'");

            var argDict = new Dictionary<string, string>();
            for (int i = 0; i < arguments.Count; i++)
                argDict.Add(targetAction.Parameters.Values[i].Name, arguments[i].Name);

            var allPrediates = targetAction.Effects.FindTypes<PredicateExp>();

            foreach (var predicate in allPrediates)
                ExecuteEffect(predicate, argDict);
        }

        private void ExecuteEffect(INode node, Dictionary<string, string> dict)
        {
            if (node is PredicateExp predicate)
            {
                var op = new Operator(predicate.Name, GroundArguments(predicate.Arguments, dict));
                if (predicate.Parent is NotExp)
                    State.Remove(op);
                else
                    State.Add(op);
            }
        }

        private List<OperatorObject> GroundArguments(List<NameExp> args, Dictionary<string, string> dict)
        {
            List<OperatorObject> newObjects = new List<OperatorObject>();

            foreach (var arg in args)
                newObjects.Add(new OperatorObject(dict[arg.Name], arg.Type.Name));

            return newObjects;
        }
    }
}
