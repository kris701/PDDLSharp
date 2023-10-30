using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Models.SAS;

namespace PDDLSharp.Toolkit.Planners.Tools
{
    public class Layer
    {
        public HashSet<Operator> Operators { get; set; }
        public HashSet<Fact> Propositions { get; set; }

        public Layer(HashSet<Operator> actions, HashSet<Fact> propositions)
        {
            Operators = actions;
            Propositions = propositions;
        }

        public Layer()
        {
            Operators = new HashSet<Operator>();
            Propositions = new HashSet<Fact>();
        }
    }
}
