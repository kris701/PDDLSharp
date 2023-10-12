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

        // The other is important!
        // Based on: https://stackoverflow.com/a/30758270
        public override int GetHashCode()
        {
            const int seed = 487;
            const int modifier = 31;
            unchecked
            {
                return Cost * Plan.Aggregate(seed, (current, item) =>
                    (current * modifier) + item.GetHashCode());
            }
        }

    }
}
