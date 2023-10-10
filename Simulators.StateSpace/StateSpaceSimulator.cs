using PDDLSharp.Contextualisers;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.PDDL;
using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.Plans;
using PDDLSharp.States.PDDL;

namespace PDDLSharp.Simulators.StateSpace
{
    public class StateSpaceSimulator : IStateSpaceSimulator
    {
        public PDDLDecl Declaration { get; internal set; }
        public IPDDLState State { get; internal set; }
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

            State = new PDDLStateSpace(declaration);
        }

        public void ExecutePlan(ActionPlan plan)
        {
            foreach (var step in plan.Plan)
            {
                var argStr = new List<string>();
                foreach (var arg in step.Arguments)
                    argStr.Add(arg.Name);

                Step(step.ActionName, NameExpBuilder.GetNameExpFromString(argStr.ToArray(), Declaration));
            }
        }

        public void Reset()
        {
            Cost = 0;
            State = new PDDLStateSpace(Declaration);
        }

        public void Step(string actionName, params string[] arguments)
        {
            if (Declaration.Problem.Objects == null)
                throw new ArgumentException("Objects not declared in problem");
            Step(actionName, NameExpBuilder.GetNameExpFromString(arguments, Declaration));
        }

        public void Step(string actionName) => Step(actionName, new List<NameExp>());

        private void Step(string actionName, List<NameExp> arguments)
        {
            actionName = actionName.ToLower();

            var targetAction = GetTargetAction(actionName).Copy(null);

            if (targetAction.Parameters.Values.Count != arguments.Count)
                throw new ArgumentOutOfRangeException($"Action takes '{targetAction.Parameters.Values.Count}' arguments, but was given '{arguments.Count}'");

            targetAction = GroundAction(targetAction, arguments);

            if (!State.IsNodeTrue(targetAction.Preconditions))
                throw new ArgumentException("Not all precondition predicates are set!");

            State.ExecuteNode(targetAction.Effects);

            Cost++;
        }

        private ActionDecl GetTargetAction(string actionName)
        {
            var targetAction = Declaration.Domain.Actions.FirstOrDefault(x => x.Name == actionName);
            if (targetAction == null)
                throw new ArgumentNullException($"Could not find an action called '{actionName}'");
            return targetAction;
        }

        public bool IsInGoal()
        {
            if (Declaration.Problem.Goal == null)
                throw new ArgumentException("Goal not set!");
            return State.IsNodeTrue(Declaration.Problem.Goal.GoalExp);
        }

        private ActionDecl GroundAction(ActionDecl node, List<NameExp> groundArgs)
        {
            for(int i = 0; i < node.Parameters.Values.Count; i++)
            {
                var names = node.FindNames(node.Parameters.Values[i].Name);
                foreach (var name in names)
                    name.Name = groundArgs[i].Name;
            }
            return node;
        }
    }
}
