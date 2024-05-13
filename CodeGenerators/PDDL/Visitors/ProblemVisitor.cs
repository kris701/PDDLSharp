using PDDLSharp.Models.PDDL.Problem;
using System.Text;

namespace PDDLSharp.CodeGenerators.Visitors
{
    public partial class GeneratorVisitors
    {
        public string Visit(ProblemDecl node, int indent)
        {
            if (node.IsHidden)
                return "";

            _printTypeOverride = false;
            if (node.Objects == null)
                _printTypeOverride = true;
            else if (node.Objects.Objs.All(x => x.Type.Name == "object"))
                _printTypeOverride = true;

            var retStr = new StringBuilder($"{IndentStr(indent)}(define{Environment.NewLine}");
            if (node.Name != null)
                retStr.AppendLine(Visit(node.Name, indent + 1));
            if (node.DomainName != null)
                retStr.AppendLine(Visit(node.DomainName, indent + 1));
            if (node.Requirements != null)
                retStr.AppendLine(Visit(node.Requirements, indent + 1));
            if (node.Objects != null)
                retStr.AppendLine(Visit(node.Objects, indent + 1));
            if (node.Init != null)
                retStr.AppendLine(Visit(node.Init, indent + 1));
            if (node.Goal != null)
                retStr.AppendLine(Visit(node.Goal, indent + 1));
            if (node.Metric != null)
                retStr.AppendLine(Visit(node.Metric, indent + 1));

            retStr.AppendLine($"{IndentStr(indent)})");

            _printTypeOverride = false;

            return retStr.ToString();
        }

        public string Visit(DomainNameRefDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}(:domain {node.Name}){Environment.NewLine}";
        }

        public string Visit(InitDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:init{Environment.NewLine}");
            foreach (var predicate in node.Predicates)
                retStr.AppendLine(Visit((dynamic)predicate, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(GoalDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:goal{Environment.NewLine}");
            retStr.AppendLine(Visit((dynamic)node.GoalExp, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(MetricDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:metric{Environment.NewLine}");
            retStr.AppendLine($"{IndentStr(indent + 1)}{node.MetricType}");
            retStr.AppendLine(Visit((dynamic)node.MetricExp, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(ObjectsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:objects{Environment.NewLine}");
            PrintTypes(true);
            foreach (var obj in node.Objs)
                retStr.AppendLine(Visit(obj, indent + 1).Replace("(", "").Replace(")", ""));
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(ProblemNameDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}(problem {node.Name}){Environment.NewLine}";
        }
    }
}
