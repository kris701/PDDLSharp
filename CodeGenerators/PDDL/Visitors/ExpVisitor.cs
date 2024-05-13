using PDDLSharp.Models.PDDL.Expressions;
using System.Text;

namespace PDDLSharp.CodeGenerators.Visitors
{
    public partial class GeneratorVisitors
    {
        public string Visit(NameExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            if (_printType)
            {
                if (node.Type == null || node.Type.Name == "")
                    return $"{IndentStr(indent)}({node.Name}) - object";
                else
                    return $"{IndentStr(indent)}({node.Name} - {Visit(node.Type, 0)})";
            }
            else
                return $"{IndentStr(indent)}({node.Name})";
        }

        public string Visit(AndExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(and");
            if (node.Children.Count > 1)
            {
                retStr.AppendLine();
                foreach (var type in node.Children)
                    retStr.AppendLine(Visit((dynamic)type, indent + 1));
                retStr.AppendLine($"{IndentStr(indent)})");
            }
            else if (node.Children.Count == 1)
                retStr.AppendLine($" {Visit((dynamic)node.Children[0], 0)})");
            else if (node.Children.Count == 0)
                retStr.AppendLine(")");

            return retStr.ToString();
        }

        public string Visit(ImplyExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(imply{Environment.NewLine}");
            retStr.AppendLine(Visit((dynamic)node.Antecedent, indent + 1));
            retStr.AppendLine(Visit((dynamic)node.Consequent, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(WhenExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(when{Environment.NewLine}");
            retStr.AppendLine(Visit((dynamic)node.Condition, indent + 1));
            retStr.AppendLine(Visit((dynamic)node.Effect, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(ParameterExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder();
            foreach (var type in node.Values)
                retStr.Append($" {Visit(type, 0)}".Replace("(", "").Replace(")", ""));
            return $"({retStr.ToString().Trim()})";
        }

        public string Visit(ForAllExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            PrintTypes(true);
            var retStr = new StringBuilder($"{IndentStr(indent)}(forall {Visit((dynamic)node.Parameters, indent + 1)}{Environment.NewLine}");
            PrintTypes(false);
            retStr.AppendLine(Visit((dynamic)node.Expression, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(ExistsExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            PrintTypes(true);
            var retStr = new StringBuilder($"{IndentStr(indent)}(exists {Visit((dynamic)node.Parameters, indent + 1)}{Environment.NewLine}");
            PrintTypes(false);
            retStr.AppendLine(Visit((dynamic)node.Expression, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(NotExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(not");
            if (node.Child is PredicateExp)
            {
                retStr.Append($" {Visit((dynamic)node.Child, 0)})");
            }
            else
            {
                retStr.AppendLine();
                retStr.AppendLine(Visit((dynamic)node.Child, indent + 1));
                retStr.Append($"{IndentStr(indent)})");
            }
            return retStr.ToString();
        }

        public string Visit(NumericExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var numericValue = "";
            if (node.Arg2 is LiteralExp)
                numericValue = $"{Visit((dynamic)node.Arg2, 0)}".Replace("(", "").Replace(")", "").Trim();
            else
                numericValue = $"{Visit((dynamic)node.Arg2, 0)}".Trim();
            return $"{IndentStr(indent)}({node.Name} {Visit((dynamic)node.Arg1, 0)} {numericValue})";
        }

        public string Visit(LiteralExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}{node.Value})";
        }

        public string Visit(TimedLiteralExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}(at {node.Value} {Visit((dynamic)node.Literal, 0)})";
        }

        public string Visit(OrExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(or{Environment.NewLine}");
            foreach (var option in node.Options)
                retStr.AppendLine(Visit((dynamic)option, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(PredicateExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}({node.Name}");
            foreach (var arg in node.Arguments)
                retStr.Append($" {$"{Visit((dynamic)arg, 0)}".Replace("(", "").Replace(")", "")}");
            retStr.Append(")");
            return retStr.ToString();
        }

        public string Visit(TypeExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}{node.Name}";
        }
    }
}
