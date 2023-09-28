using Microsoft.VisualBasic;
using PDDLSharp.ErrorListeners;
using PDDLSharp.Models.Domain;
using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PDDLSharp.CodeGenerators.Visitors
{
    public partial class GeneratorVisitors
    {
        public string Visit(DomainDecl node)
        {
            string retStr = "(define ";
            retStr += $"{Visit(node.Name)}{Environment.NewLine}";
            if (node.Requirements != null)
                retStr += $"{Visit(node.Requirements)}{Environment.NewLine}";
            if (node.Types != null)
                retStr += $"{Visit(node.Types)}{Environment.NewLine}";
            if (node.Predicates != null)
                retStr += $"{Visit(node.Predicates)}{Environment.NewLine}";
            if (node.Constants != null)
                retStr += $"{Visit(node.Constants)}{Environment.NewLine}";
            if (node.Extends != null)
                retStr += $"{Visit(node.Extends)}{Environment.NewLine}";
            if (node.Timeless != null)
                retStr += $"{Visit(node.Timeless)}{Environment.NewLine}";
            if (node.Functions != null)
                retStr += $"{Visit(node.Functions)}{Environment.NewLine}";

            if (node.Actions != null)
                foreach(var act in node.Actions)
                    retStr += $"{Visit(act)}{Environment.NewLine}";
            if (node.Axioms != null)
                foreach (var axi in node.Axioms)
                    retStr += $"{Visit(axi)}{Environment.NewLine}";
            if (node.DurativeActions != null)
                foreach (var durAct in node.DurativeActions)
                    retStr += $"{Visit(durAct)}{Environment.NewLine}";

            retStr += ")";
            return retStr;
        }

        public string Visit(ActionDecl node)
        {
            string retStr = $"(:action {node.Name}{Environment.NewLine}";
            retStr += $":parameters {Visit(node.Parameters)}{Environment.NewLine}";
            retStr += $":precondition {Visit((dynamic)node.Preconditions)}{Environment.NewLine}";
            retStr += $":effect {Visit((dynamic)node.Effects)}{Environment.NewLine}";
            retStr += $"){Environment.NewLine}";
            return retStr;
        }

        public string Visit(AxiomDecl node)
        {
            string retStr = $"(:axiom{Environment.NewLine}";
            retStr += $":vars {Visit(node.Vars)}{Environment.NewLine}";
            retStr += $":context {Visit((dynamic)node.Context)}{Environment.NewLine}";
            retStr += $":implies {Visit((dynamic)node.Implies)}{Environment.NewLine}";
            retStr += $"){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ConstantsDecl node)
        {
            string retStr = "";
            foreach (var type in node.Constants)
                retStr += $" {Visit(type)}{Environment.NewLine}".Replace("(","").Replace(")","");
            return $"(:constants{retStr}){Environment.NewLine}";
        }

        public string Visit(DomainNameDecl node)
        {
            return $"(domain {node.Name}){Environment.NewLine}";
        }

        public string Visit(DurativeActionDecl node)
        {
            string retStr = $"(:durative-action {node.Name}{Environment.NewLine}";
            retStr += $":parameters {Visit(node.Parameters)}{Environment.NewLine}";
            retStr += $":duration {Visit((dynamic)node.Duration)}{Environment.NewLine}";
            retStr += $":precondition {Visit((dynamic)node.Condition)}{Environment.NewLine}";
            retStr += $":effect {Visit((dynamic)node.Effects)}{Environment.NewLine}";
            retStr += $"){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ExtendsDecl node)
        {
            string retStr = "";
            foreach (var type in node.Extends)
                retStr += $" {Visit(type)}{Environment.NewLine}";
            return $"(:extends{retStr}{Environment.NewLine})";
        }

        public string Visit(FunctionsDecl node)
        {
            string retStr = "";
            foreach (var type in node.Functions)
                retStr += $" {Visit(type)}{Environment.NewLine}";
            return $"(:functions{retStr}{Environment.NewLine})";
        }

        public string Visit(ParameterDecl node)
        {
            string retStr = "";
            foreach (var type in node.Values)
                retStr += $" {Visit(type)}".Replace("(","").Replace(")","");
            return $"({retStr})";
        }

        public string Visit(PredicatesDecl node)
        {
            string retStr = "";
            foreach (var type in node.Predicates)
                retStr += $" {Visit(type)}{Environment.NewLine}";
            return $"(:predicates{retStr}){Environment.NewLine}";
        }

        public string Visit(RequirementsDecl node)
        {
            var reqStr = "";
            foreach (var requirement in node.Requirements)
                reqStr += $" {requirement.Name}";
            return $"(:requirements{reqStr}){Environment.NewLine}";
        }

        public string Visit(TimelessDecl node)
        {
            string retStr = "";
            foreach (var type in node.Items)
                retStr += $" {Visit(type)}{Environment.NewLine}";
            return $"(:timeless{retStr}){Environment.NewLine}";
        }

        public string Visit(TypesDecl node)
        {
            string retStr = "";
            foreach (var type in node.Types)
            {
                if (type.SuperType != "")
                {
                    retStr += $" {Visit(type)} - {type.SuperType}{Environment.NewLine}";
                }
                else
                {
                    retStr += $" {Visit(type)}{Environment.NewLine}";
                }
            }
            return $"(:types{retStr}){Environment.NewLine}";
        }
    }
}
