using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class EmptyExp : BaseNode, IExp
    {
        public override INode Copy(INode? newParent = null)
        {
            return new EmptyExp();
        }

        public override bool Equals(object? obj)
        {
            if (obj is EmptyExp other)
            {
                if (!base.Equals(other)) return false;
                return true;
            }
            return false;
        }


        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
