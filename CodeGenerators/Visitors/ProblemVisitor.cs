using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using PDDLSharp.Models.PDDL.Problem;
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
        public string Visit(ProblemDecl node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(define{Environment.NewLine}";
            if (node.Name != null)
                retStr += $"{Visit(node.Name, indent + 1)}{Environment.NewLine}";
            if (node.DomainName != null)
                retStr += $"{Visit(node.DomainName, indent + 1)}{Environment.NewLine}";
            if (node.Objects != null)
                retStr += $"{Visit(node.Objects, indent + 1)}{Environment.NewLine}";
            if (node.Init != null)
                retStr += $"{Visit(node.Init, indent + 1)}{Environment.NewLine}";
            if (node.Goal != null)
                retStr += $"{Visit(node.Goal, indent + 1)}{Environment.NewLine}";
            if (node.Metric != null)
                retStr += $"{Visit(node.Metric, indent + 1)}{Environment.NewLine}";

            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(DomainNameRefDecl node, int indent)
        {
            return $"{IndentStr(indent)}(:domain {node.Name}){Environment.NewLine}";
        }

        public string Visit(InitDecl node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(:init{Environment.NewLine}";
            foreach (var predicate in node.Predicates)
                retStr += $"{Visit((dynamic)predicate, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(GoalDecl node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(:goal{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.GoalExp, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(MetricDecl node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(:metric{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}{node.MetricType}{Environment.NewLine}";
            retStr += $"{Visit((dynamic)node.MetricExp, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ObjectsDecl node, int indent)
        {
            string retStr = $"{IndentStr(indent)}(:objects{Environment.NewLine}";
            _printType = true;
            foreach (var obj in node.Objs)
                retStr += $"{Visit(obj, indent + 1)}{Environment.NewLine}".Replace("(", "").Replace(")", "");
            _printType = false;
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ProblemNameDecl node, int indent)
        {
            return $"{IndentStr(indent)}(problem {node.Name}){Environment.NewLine}";
        }
    }
}
