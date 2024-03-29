﻿using PDDLSharp.Models.PDDL.Expressions;

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
            string retStr = $"{IndentStr(indent)}(and";
            if (node.Children.Count > 1)
            {
                retStr += Environment.NewLine;
                foreach (var type in node.Children)
                    retStr += $"{Visit((dynamic)type, indent + 1)}{Environment.NewLine}";
                retStr += $"{IndentStr(indent)})";
            }
            else if (node.Children.Count == 1)
                retStr += $" {Visit((dynamic)node.Children[0], 0)}){Environment.NewLine}";
            else if (node.Children.Count == 0)
                retStr += $"){Environment.NewLine}";

            return retStr;
        }

        public string Visit(ImplyExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(imply{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Antecedent, indent + 1)}{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Consequent, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(WhenExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(when{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Condition, indent + 1)}{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Effect, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ParameterExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = "";
            foreach (var type in node.Values)
                retStr += $" {Visit(type, 0)}".Replace("(", "").Replace(")", "");
            return $"({retStr.Trim()})";
        }

        public string Visit(ForAllExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            PrintTypes(true);
            string retStr = $"{IndentStr(indent)}(forall {Visit((dynamic)node.Parameters, indent + 1)}{Environment.NewLine}";
            PrintTypes(false);
            retStr += $"{Visit((dynamic)node.Expression, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ExistsExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            PrintTypes(true);
            string retStr = $"{IndentStr(indent)}(exists {Visit((dynamic)node.Parameters, indent + 1)}{Environment.NewLine}";
            PrintTypes(false);
            retStr += $"{Visit((dynamic)node.Expression, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(NotExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(not";
            if (node.Child is PredicateExp)
            {
                retStr += $" {Visit((dynamic)node.Child, 0)})";
            }
            else
            {
                retStr += Environment.NewLine;
                retStr += $"{Visit((dynamic)node.Child, indent + 1)}{Environment.NewLine}";
                retStr += $"{IndentStr(indent)})";
            }
            return retStr;
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
            string retStr = $"{IndentStr(indent)}(or{Environment.NewLine}";
            foreach (var option in node.Options)
                retStr += $"{Visit((dynamic)option, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(PredicateExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}({node.Name}";
            foreach (var arg in node.Arguments)
            {
                var argStr = $"{Visit((dynamic)arg, 0)}".Replace("(", "").Replace(")", "");
                retStr += $" {argStr}";
            }
            retStr += ")";
            return retStr;
        }

        public string Visit(TypeExp node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}{node.Name}";
        }
    }
}
