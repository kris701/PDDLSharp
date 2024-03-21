using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.FastDownward.Plans;

namespace PDDLSharp.CodeGenerators.FastDownward.Plans
{
    public class FastDownwardPlanGenerator : ICodeGenerator<ActionPlan>
    {
        public IErrorListener Listener { get; }
        public bool Readable { get; set; } = false;

        public FastDownwardPlanGenerator(IErrorListener listener)
        {
            Listener = listener;
        }

        public void Generate(ActionPlan node, string toFile) => File.WriteAllText(toFile, Generate(node));
        public string Generate(ActionPlan node)
        {
            var retStr = "";

            foreach (var act in node.Plan)
            {
                var argStr = "";
                foreach (var arg in act.Arguments)
                    argStr += $" {arg.Name}";
                retStr += $"({act.ActionName}{argStr}){Environment.NewLine}";
            }
            retStr += $"; cost = {node.Cost} (unit cost)";

            return retStr;
        }
    }
}
