using PDDLSharp.Models.FastDownward.SAS;
using PDDLSharp.Models.FastDownward.SAS.Sections;

namespace PDDLSharp.CodeGenerators.FastDownward.SAS
{
    public class SectionVisitor
    {
        public string Visit(SASDecl node)
        {
            var retStr = "";
            if (node.Version != null)
                retStr += $"{Visit(node.Version)}{Environment.NewLine}";
            if (node.Metric != null)
                retStr += $"{Visit(node.Metric)}{Environment.NewLine}";
            foreach (var variable in node.Variables)
                retStr += $"{Visit(variable)}{Environment.NewLine}";
            foreach (var mutex in node.Mutexes)
                retStr += $"{Visit(mutex)}{Environment.NewLine}";
            if (node.InitState != null)
                retStr += $"{Visit(node.InitState)}{Environment.NewLine}";
            if (node.GoalState != null)
                retStr += $"{Visit(node.GoalState)}{Environment.NewLine}";
            foreach (var op in node.Operators)
                retStr += $"{Visit(op)}{Environment.NewLine}";
            foreach (var axiom in node.Axioms)
                retStr += $"{Visit(axiom)}{Environment.NewLine}";

            return retStr;
        }

        public string Visit(AxiomDecl node)
        {
            var retStr = $"begin_rule{Environment.NewLine}";
            retStr += $"{node.Conditions.Count}{Environment.NewLine}";
            foreach (var group in node.Conditions)
                retStr += $"{group}{Environment.NewLine}";
            retStr += $"{node.EffectedVariable} {node.VariablePrecondition} {node.NewVariableValue}{Environment.NewLine}";
            retStr += $"end_rule";
            return retStr;
        }

        public string Visit(GoalStateDecl node)
        {
            var retStr = $"begin_goal{Environment.NewLine}";
            retStr += $"{node.Goals.Count}{Environment.NewLine}";
            foreach (var group in node.Goals)
                retStr += $"{group}{Environment.NewLine}";
            retStr += $"end_goal";
            return retStr;
        }

        public string Visit(InitStateDecl node)
        {
            var retStr = $"begin_state{Environment.NewLine}";
            foreach (var group in node.Inits)
                retStr += $"{group}{Environment.NewLine}";
            retStr += $"end_state";
            return retStr;
        }

        public string Visit(MetricDecl node)
        {
            var retStr = $"begin_metric{Environment.NewLine}";
            if (node.IsUsingMetrics)
                retStr += $"1{Environment.NewLine}";
            else
                retStr += $"0{Environment.NewLine}";
            retStr += $"end_metric";
            return retStr;
        }

        public string Visit(MutexDecl node)
        {
            var retStr = $"begin_mutex_group{Environment.NewLine}";
            retStr += $"{node.Group.Count}{Environment.NewLine}";
            foreach (var group in node.Group)
                retStr += $"{group}{Environment.NewLine}";
            retStr += $"end_mutex_group";
            return retStr;
        }

        public string Visit(OperatorDecl node)
        {
            var retStr = $"begin_operator{Environment.NewLine}";
            retStr += $"{node.Name}{Environment.NewLine}";
            retStr += $"{node.PrevailConditions.Count}{Environment.NewLine}";
            foreach (var group in node.PrevailConditions)
                retStr += $"{group}{Environment.NewLine}";
            retStr += $"{node.Effects.Count}{Environment.NewLine}";
            foreach (var group in node.Effects)
                retStr += $"{group}{Environment.NewLine}";
            retStr += $"{node.Cost}{Environment.NewLine}";
            retStr += $"end_operator";
            return retStr;
        }

        public string Visit(VariableDecl node)
        {
            var retStr = $"begin_variable{Environment.NewLine}";
            retStr += $"{node.VariableName}{Environment.NewLine}";
            retStr += $"{node.AxiomLayer}{Environment.NewLine}";
            retStr += $"{node.SymbolicNames.Count}{Environment.NewLine}";
            foreach (var sym in node.SymbolicNames)
                retStr += $"{sym}{Environment.NewLine}";
            retStr += $"end_variable";
            return retStr;
        }

        public string Visit(VersionDecl node)
        {
            var retStr = $"begin_version{Environment.NewLine}";
            retStr += $"{node.Version}{Environment.NewLine}";
            retStr += $"end_version";
            return retStr;
        }
    }
}
