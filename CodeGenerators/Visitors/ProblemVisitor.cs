using PDDL.ErrorListeners;
using PDDL.Models.Domain;
using PDDL.Models.Expressions;
using PDDL.Models.Problem;
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
        public string Visit(ProblemDecl node)
        {
            string retStr = "(define ";
            retStr += $"{Visit(node.Name)}{Environment.NewLine}";
            if (node.DomainName != null)
                retStr += $"{Visit(node.DomainName)}{Environment.NewLine}";
            if (node.Objects != null)
                retStr += $"{Visit(node.Objects)}{Environment.NewLine}";
            if (node.Init != null)
                retStr += $"{Visit(node.Init)}{Environment.NewLine}";
            if (node.Goal != null)
                retStr += $"{Visit(node.Goal)}{Environment.NewLine}";
            if (node.Metric != null)
                retStr += $"{Visit(node.Metric)}{Environment.NewLine}";

            retStr += ")";
            return retStr;
        }

        public string Visit(DomainNameRefDecl node)
        {
            return $"(:domain {node.Name}){Environment.NewLine}";
        }

        public string Visit(InitDecl node)
        {
            string retStr = "";
            foreach (var type in node.Predicates)
                retStr += $" {Visit((dynamic)type)}{Environment.NewLine}";
            return $"(:init{retStr}){Environment.NewLine}";
        }

        public string Visit(GoalDecl node)
        {
            return $"(:goal {Visit((dynamic)node.GoalExp)}){Environment.NewLine}";
        }

        public string Visit(MetricDecl node)
        {
            return $"(:metric {node.MetricType} {Visit((dynamic)node.MetricExp)}){Environment.NewLine}";
        }

        public string Visit(ObjectsDecl node)
        {
            string retStr = "";
            foreach (var type in node.Objs)
                retStr += $" {Visit((dynamic)type)}{Environment.NewLine}";
            return $"(:objects{retStr}){Environment.NewLine}";
        }

        public string Visit(ProblemNameDecl node)
        {
            return $"(problem {node.Name}){Environment.NewLine}";
        }
    }
}
