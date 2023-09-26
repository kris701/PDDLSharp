using PDDL.ErrorListeners;
using PDDL.Models;
using PDDL.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PDDL.CodeGenerators.Visitors
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
            return $"(not {node.Child})";
        }

        public string Visit(NumericExp node)
        {
            return $"({node.Name} {node.Arg1} {node.Arg2})";
        }

        public string Visit(OrExp node)
        {
            return $"(or {node.Option1} {node.Option2})";
        }

        public string Visit(PredicateExp node)
        {
            var paramRetStr = "";
            foreach (var arg in node.Arguments)
                paramRetStr += $" {arg}";
            return $"({node.Name}{paramRetStr})";
        }

        public string Visit(TypeExp node)
        {
            return node.Name;
        }
    }
}
