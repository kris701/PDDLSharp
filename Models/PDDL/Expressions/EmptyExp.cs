namespace PDDLSharp.Models.PDDL.Expressions
{
    public class EmptyExp : BaseNode, IExp
    {
        public override INode Copy(INode? newParent = null)
        {
            return new EmptyExp();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
