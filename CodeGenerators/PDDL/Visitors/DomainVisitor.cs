using PDDLSharp.Models.PDDL.Domain;
using PDDLSharp.Models.PDDL.Shared;
using System.Text;

namespace PDDLSharp.CodeGenerators.Visitors
{
    public partial class GeneratorVisitors
    {
        public string Visit(DomainDecl node, int indent)
        {
            if (node.IsHidden)
                return "";

            if (node.Types == null)
                _printTypeOverride = true;

            var retStr = new StringBuilder($"{IndentStr(indent)}(define{Environment.NewLine}");
            if (node.Name != null)
                retStr.AppendLine(Visit(node.Name, indent + 1));
            if (node.Extends != null)
                retStr.AppendLine(Visit(node.Extends, indent + 1));
            if (node.Requirements != null)
                retStr.AppendLine(Visit(node.Requirements, indent + 1));
            if (node.Types != null)
                retStr.AppendLine(Visit(node.Types, indent + 1));
            if (node.Constants != null)
                retStr.AppendLine(Visit(node.Constants, indent + 1));
            if (node.Predicates != null)
                retStr.AppendLine(Visit(node.Predicates, indent + 1));
            if (node.Timeless != null)
                retStr.AppendLine(Visit(node.Timeless, indent + 1));
            if (node.Functions != null)
                retStr.AppendLine(Visit(node.Functions, indent + 1));

            if (node.Actions != null)
                foreach (var act in node.Actions)
                    retStr.AppendLine(Visit(act, indent + 1));
            if (node.Deriveds != null)
                foreach (var act in node.Deriveds)
                    retStr.AppendLine(Visit(act, indent + 1));
            if (node.Axioms != null)
                foreach (var axi in node.Axioms)
                    retStr.AppendLine(Visit(axi, indent + 1));
            if (node.DurativeActions != null)
                foreach (var durAct in node.DurativeActions)
                    retStr.AppendLine(Visit(durAct, indent + 1));

            retStr.AppendLine($"{IndentStr(indent)})");

            _printTypeOverride = false;

            return retStr.ToString();
        }

        public string Visit(ActionDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:action {node.Name}{Environment.NewLine}");
            PrintTypes(true);
            retStr.AppendLine($"{IndentStr(indent + 1)}:parameters {Visit(node.Parameters, 0)}");
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent + 1)}:precondition {Environment.NewLine}{Visit((dynamic)node.Preconditions, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent + 1)}:effect {Environment.NewLine}{Visit((dynamic)node.Effects, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(AxiomDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:axiom{Environment.NewLine}");
            PrintTypes(true);
            retStr.AppendLine($"{IndentStr(indent + 1)}:vars {Visit(node.Parameters, indent + 2)}");
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent + 1)}:context {Visit((dynamic)node.Context, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent + 1)}:implies {Visit((dynamic)node.Implies, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(DurativeActionDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:durative-action {node.Name}{Environment.NewLine}");
            PrintTypes(true);
            retStr.AppendLine($"{IndentStr(indent + 1)}:parameters {Visit(node.Parameters, indent + 2)}");
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent + 1)}:duration {Visit((dynamic)node.Duration, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent + 1)}:condition {Visit((dynamic)node.Condition, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent + 1)}:effect {Visit((dynamic)node.Effects, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(DerivedDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:derived{Environment.NewLine}");
            PrintTypes(true);
            retStr.AppendLine($"{IndentStr(indent + 1)}{Visit(node.Predicate, indent + 2)}");
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent + 1)}{Visit((dynamic)node.Expression, indent + 2)}");
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(ConstantsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:constants{Environment.NewLine}");
            PrintTypes(true);
            foreach (var constant in node.Constants)
                retStr.AppendLine(Visit((dynamic)constant, indent + 1).Replace("(", "").Replace(")", ""));
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(DomainNameDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            return $"{IndentStr(indent)}(domain {node.Name}){Environment.NewLine}";
        }

        public string Visit(ExtendsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:extends{Environment.NewLine}");
            foreach (var extends in node.Extends)
                retStr.AppendLine(Visit((dynamic)extends, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(FunctionsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:functions{Environment.NewLine}");
            PrintTypes(true);
            foreach (var function in node.Functions)
                retStr.AppendLine(Visit((dynamic)function, indent + 1));
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(PredicatesDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:predicates{Environment.NewLine}");
            PrintTypes(true);
            foreach (var predicate in node.Predicates)
                retStr.AppendLine(Visit((dynamic)predicate, indent + 1));
            PrintTypes(false);
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(RequirementsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:requirements");
            foreach (var requirement in node.Requirements)
                retStr.Append($" {requirement.Name}");
            retStr.AppendLine(")");
            return retStr.ToString();
        }

        public string Visit(TimelessDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:timeless{Environment.NewLine}");
            foreach (var timelessItem in node.Items)
                retStr.AppendLine(Visit((dynamic)timelessItem, indent + 1));
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }

        public string Visit(TypesDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            var retStr = new StringBuilder($"{IndentStr(indent)}(:types{Environment.NewLine}");
            foreach (var type in node.Types)
            {
                if (type.SuperType != "domain")
                {
                    retStr.AppendLine($"{Visit(type, indent + 1)} - {type.SuperType}");
                }
                else
                {
                    retStr.AppendLine(Visit(type, indent + 1));
                }
            }
            retStr.AppendLine($"{IndentStr(indent)})");
            return retStr.ToString();
        }
    }
}
