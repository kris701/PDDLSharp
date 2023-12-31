﻿using PDDLSharp.Models.PDDL.Problem;

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

            string retStr = $"{IndentStr(indent)}(define{Environment.NewLine}";
            if (node.Name != null)
                retStr += $"{Visit(node.Name, indent + 1)}{Environment.NewLine}";
            if (node.DomainName != null)
                retStr += $"{Visit(node.DomainName, indent + 1)}{Environment.NewLine}";
            if (node.Requirements != null)
                retStr += $"{Visit(node.Requirements, indent + 1)}{Environment.NewLine}";
            if (node.Objects != null)
                retStr += $"{Visit(node.Objects, indent + 1)}{Environment.NewLine}";
            if (node.Init != null)
                retStr += $"{Visit(node.Init, indent + 1)}{Environment.NewLine}";
            if (node.Goal != null)
                retStr += $"{Visit(node.Goal, indent + 1)}{Environment.NewLine}";
            if (node.Metric != null)
                retStr += $"{Visit(node.Metric, indent + 1)}{Environment.NewLine}";

            retStr += $"{IndentStr(indent)}){Environment.NewLine}";

            _printTypeOverride = false;

            return retStr;
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
            string retStr = $"{IndentStr(indent)}(:init{Environment.NewLine}";
            foreach (var predicate in node.Predicates)
                retStr += $"{Visit((dynamic)predicate, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(GoalDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:goal{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.GoalExp, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(MetricDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:metric{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}{node.MetricType}{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.MetricExp, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ObjectsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:objects{Environment.NewLine}";
            PrintTypes(true);
            foreach (var obj in node.Objs)
                retStr += $"{Visit(obj, indent + 1)}{Environment.NewLine}".Replace("(", "").Replace(")", "");
            PrintTypes(false);
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ProblemNameDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}(problem {node.Name}){Environment.NewLine}";
        }
    }
}
