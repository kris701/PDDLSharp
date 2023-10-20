using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.Plans
{
    public class GroundedAction
    {
        public string ActionName { get; set; }
        public List<NameExp> Arguments { get; set; }

        public GroundedAction(string actionName, params string[] arguments)
        {
            ActionName = actionName;
            Arguments = new List<NameExp>();
            foreach (var arg in arguments)
                Arguments.Add(new NameExp(arg));
        }

        public GroundedAction(string actionName, List<NameExp> arguments)
        {
            ActionName = actionName;
            Arguments = arguments;
        }

        public GroundedAction(ActionDecl action, params string[] arguments)
        {
            ActionName = action.Name;
            Arguments = new List<NameExp>();
            foreach (var arg in arguments)
                Arguments.Add(new NameExp(arg));
        }

        public GroundedAction(ActionDecl action, List<NameExp> arguments)
        {
            ActionName = action.Name;
            Arguments = arguments;
        }

        public override string? ToString()
        {
            var retStr = ActionName;
            foreach (var arg in Arguments)
                retStr += $" {arg}";
            return retStr;
        }

        public override bool Equals(object? obj)
        {
            if (obj is GroundedAction op)
                return op.GetHashCode() == GetHashCode();
            return false;
        }

        public override int GetHashCode()
        {
            var hash = ActionName.GetHashCode();
            foreach (var arg in Arguments)
                hash ^= arg.GetHashCode();
            return hash;
        }
    }
}
