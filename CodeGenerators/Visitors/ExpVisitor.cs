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
        public string Visit(NameExp node)
        {
            if (node.Type == null || node.Type.Name == "")
                return $"({node.Name})";
            else
                return $"({node.Name} - {Visit(node.Type)})";
        }

        public string Visit(AndExp node)
        {
            string retStr = "";
            foreach (var type in node.Children)
                retStr += $" {Visit((dynamic)type)}{Environment.NewLine}";
            return $"(and{retStr})";
        }

        public string Visit(NotExp node)
        {
            return $"(not {Visit((dynamic)node.Child)})";
        }

        public string Visit(NumericExp node)
        {
            var numericValue = $"{Visit((dynamic)node.Arg2)}".Replace("(","").Replace(")","");
            return $"({node.Name} {Visit((dynamic)node.Arg1)} {numericValue})";
        }

        public string Visit(OrExp node)
        {
            return $"(or {Visit((dynamic)node.Option1)} {Visit((dynamic)node.Option2)})";
        }

        public string Visit(PredicateExp node)
        {
            var paramRetStr = "";
            foreach (var arg in node.Arguments)
            {
                var argStr = $"{Visit((dynamic)arg)}".Replace("(","").Replace(")","");
                paramRetStr += $" {argStr}";
            }
            return $"({node.Name}{paramRetStr})";
        }

        public string Visit(TypeExp node)
        {
            return node.Name;
        }
    }
}
