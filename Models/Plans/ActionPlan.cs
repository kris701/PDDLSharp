namespace PDDLSharp.Models.Plans
{
    public class ActionPlan
    {
        public List<GroundedAction> Plan { get; set; }
        public int Cost { get; set; }

        public ActionPlan(List<GroundedAction> plan, int cost)
        {
            Plan = plan;
            Cost = cost;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ActionPlan op)
                return op.GetHashCode() == GetHashCode();
            return false;
        }

        public override int GetHashCode()
        {
            var hash = Cost;
            foreach (var arg in Plan)
                hash ^= arg.GetHashCode();
            return hash;
        }
    }
}
