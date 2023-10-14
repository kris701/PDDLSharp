using PDDLSharp.Models.PDDL.Expressions;

namespace PDDLSharp.Models.PDDL
{
    public interface IParametized : INode
    {
        public ParameterExp Parameters { get; set; }
    }
}
