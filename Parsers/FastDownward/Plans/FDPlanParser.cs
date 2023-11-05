using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;

namespace PDDLSharp.Parsers.FastDownward.Plans
{
    public class FDPlanParser : BaseParser<ActionPlan>
    {
        public FDPlanParser(IErrorListener listener) : base(listener)
        {
        }

        public override U ParseAs<U>(string text)
        {
            var plan = new List<GroundedAction>();
            int cost = 0;
            var lines = text.Split(Environment.NewLine);
            foreach (var line in lines)
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
            return (U)new ActionPlan(plan, cost);
        }
    }
}
