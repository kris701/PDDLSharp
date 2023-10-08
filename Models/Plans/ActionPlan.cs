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
    }
}
