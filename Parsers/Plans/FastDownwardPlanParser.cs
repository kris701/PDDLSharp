using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.Plans;

namespace PDDLSharp.Parsers.Plans
{
    public class FastDownwardPlanParser : BaseParser<ActionPlan>
    {
        public FastDownwardPlanParser(IErrorListener listener) : base(listener)
        {
        }

        public override ActionPlan Parse(string file)
        {
            var plan = new List<GroundedAction>();
            int cost = 0;
            foreach (var line in File.ReadAllLines(file))
            {
                if (!line.StartsWith(";") && line.Trim() != "")
                {
                    var innerLine = line.Replace("(", "").Replace(")", "");
                    var name = innerLine.Split(' ')[0];
                    var args = new List<string>();
                    foreach (var arg in innerLine.Split(' '))
                        if (arg.Trim() != name && arg.Trim() != "")
                            args.Add(arg.Trim());
                    plan.Add(new GroundedAction(name, args.ToArray()));
                }
                else if (line.Trim().StartsWith(";"))
                    cost = int.Parse(line.Substring(line.IndexOf("=") + 1, line.IndexOf("(") - line.IndexOf("=") - 1));
            }
            return new ActionPlan(plan, cost);
        }
    }
}
