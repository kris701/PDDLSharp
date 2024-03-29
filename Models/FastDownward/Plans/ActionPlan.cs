﻿namespace PDDLSharp.Models.FastDownward.Plans
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

        public ActionPlan(List<GroundedAction> plan)
        {
            Plan = plan;
            Cost = plan.Count;
        }

        public ActionPlan()
        {
            Plan = new List<GroundedAction>();
            Cost = 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ActionPlan op)
            {
                if (Cost != op.Cost) return false;
                if (Plan.Count != op.Plan.Count) return false;
                for (int i = 0; i < Plan.Count; i++)
                    if (!Plan[i].Equals(op.Plan[i]))
                        return false;
                return true;
            }
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
                return Cost + Plan.Aggregate(seed, (current, item) =>
                    current * modifier + item.GetHashCode());
            }
        }

        public ActionPlan Copy()
        {
            var actions = new List<GroundedAction>();
            foreach (var act in Plan)
                actions.Add(act.Copy());
            return new ActionPlan(actions, Cost);
        }
    }
}
