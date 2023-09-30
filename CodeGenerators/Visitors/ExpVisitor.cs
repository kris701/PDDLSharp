using PDDLSharp.ErrorListeners;
using PDDLSharp.Models;
using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDLSharp.CodeGenerators.Visitors
{
    public partial class GeneratorVisitors
    {
        public string Visit(NameExp node, int indent)
        {
            if (node.Type == null || node.Type.Name == "")
                return $"{IndentStr(indent)}({node.Name})";
            else
                return $"{IndentStr(indent)}({node.Name} - {Visit(node.Type, 0)})";
        }

        public string Visit(AndExp node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(and{Environment.NewLine}";
            foreach (var type in node.Children)
                retStr += $"{Visit((dynamic)type, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)})";
            return retStr;
        }

        public string Visit(WhenExp node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(when{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Condition, indent + 1)}{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Effect, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(NotExp node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(not{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Child, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)})";
            return retStr;
        }

        public string Visit(NumericExp node, int indent)
        {
            var numericValue = $"{Visit((dynamic)node.Arg2, 0)}".Replace("(","").Replace(")","").Trim();
            return $"{IndentStr(indent)}({node.Name} {Visit((dynamic)node.Arg1, 0)} {numericValue})";
        }

        public string Visit(OrExp node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(when{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Option1, indent + 1)}{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.Option1, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(PredicateExp node, int indent)
        {
            string retStr = $"{IndentStr(indent)}({node.Name}";
            foreach (var arg in node.Arguments)
            {
                var argStr = $"{Visit((dynamic)arg, 0)}".Replace("(","").Replace(")","");
                retStr += $" {argStr}";
            }
            retStr += ")";
            return retStr;
        }

        public string Visit(TypeExp node, int indent)
        {
            return $"{IndentStr(indent)}{node.Name}";
        }
    }
}
