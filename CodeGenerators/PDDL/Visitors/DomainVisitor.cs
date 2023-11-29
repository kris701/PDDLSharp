using PDDLSharp.Models.PDDL.Domain;

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

            string retStr = $"{IndentStr(indent)}(define{Environment.NewLine}";
            if (node.Name != null)
                retStr += $"{Visit(node.Name, indent + 1)}{Environment.NewLine}";
            if (node.Requirements != null)
                retStr += $"{Visit(node.Requirements, indent + 1)}{Environment.NewLine}";
            if (node.Types != null)
                retStr += $"{Visit(node.Types, indent + 1)}{Environment.NewLine}";
            if (node.Constants != null)
                retStr += $"{Visit(node.Constants, indent + 1)}{Environment.NewLine}";
            if (node.Predicates != null)
                retStr += $"{Visit(node.Predicates, indent + 1)}{Environment.NewLine}";
            if (node.Extends != null)
                retStr += $"{Visit(node.Extends, indent + 1)}{Environment.NewLine}";
            if (node.Timeless != null)
                retStr += $"{Visit(node.Timeless, indent + 1)}{Environment.NewLine}";
            if (node.Functions != null)
                retStr += $"{Visit(node.Functions, indent + 1)}{Environment.NewLine}";

            if (node.Actions != null)
                foreach (var act in node.Actions)
                    retStr += $"{Visit(act, indent + 1)}{Environment.NewLine}";
            if (node.Axioms != null)
                foreach (var axi in node.Axioms)
                    retStr += $"{Visit(axi, indent + 1)}{Environment.NewLine}";
            if (node.DurativeActions != null)
                foreach (var durAct in node.DurativeActions)
                    retStr += $"{Visit(durAct, indent + 1)}{Environment.NewLine}";

            retStr += $"{IndentStr(indent)}){Environment.NewLine}";

            _printTypeOverride = false;

            return retStr;
        }

        public string Visit(ActionDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:action {node.Name}{Environment.NewLine}";
            PrintTypes(true);
            retStr += $"{IndentStr(indent + 1)}:parameters {Visit(node.Parameters, 0)}{Environment.NewLine}";
            PrintTypes(false);
            retStr += $"{IndentStr(indent + 1)}:precondition {Environment.NewLine}{Visit((dynamic)node.Preconditions, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}:effect {Environment.NewLine}{Visit((dynamic)node.Effects, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(AxiomDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:axiom{Environment.NewLine}";
            PrintTypes(true);
            retStr += $"{IndentStr(indent + 1)}:vars {Visit(node.Parameters, indent + 2)}{Environment.NewLine}";
            PrintTypes(false);
            retStr += $"{IndentStr(indent + 1)}:context {Visit((dynamic)node.Context, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}:implies {Visit((dynamic)node.Implies, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(DurativeActionDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:durative-action {node.Name}{Environment.NewLine}";
            PrintTypes(true);
            retStr += $"{IndentStr(indent + 1)}:parameters {Visit(node.Parameters, indent + 2)}{Environment.NewLine}";
            PrintTypes(false);
            retStr += $"{IndentStr(indent + 1)}:duration {Visit((dynamic)node.Duration, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}:condition {Visit((dynamic)node.Condition, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}:effect {Visit((dynamic)node.Effects, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(DerivedDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:derived{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}{Visit(node.Predicate, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent + 1)}{Visit((dynamic)node.Expression, indent + 2)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(ConstantsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:constants{Environment.NewLine}";
            PrintTypes(true);
            foreach (var constant in node.Constants)
                retStr += $"{Visit((dynamic)constant, indent + 1)}{Environment.NewLine}".Replace("(", "").Replace(")", "");
            PrintTypes(false);
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
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
            string retStr = $"{IndentStr(indent)}(:extends{Environment.NewLine}";
            foreach (var extends in node.Extends)
                retStr += $"{Visit((dynamic)extends, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(FunctionsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:functions{Environment.NewLine}";
            PrintTypes(true);
            foreach (var function in node.Functions)
                retStr += $"{Visit((dynamic)function, indent + 1)}{Environment.NewLine}";
            PrintTypes(false);
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(PredicatesDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:predicates{Environment.NewLine}";
            PrintTypes(true);
            foreach (var predicate in node.Predicates)
                retStr += $"{Visit((dynamic)predicate, indent + 1)}{Environment.NewLine}";
            PrintTypes(false);
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(RequirementsDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:requirements";
            foreach (var requirement in node.Requirements)
                retStr += $" {requirement.Name}";
            retStr += $"){Environment.NewLine}";
            return retStr;
        }

        public string Visit(TimelessDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:timeless{Environment.NewLine}";
            foreach (var timelessItem in node.Items)
                retStr += $"{Visit((dynamic)timelessItem, indent + 1)}{Environment.NewLine}";
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }

        public string Visit(TypesDecl node, int indent)
        {
            if (node.IsHidden)
                return "";
            string retStr = $"{IndentStr(indent)}(:types{Environment.NewLine}";
            foreach (var type in node.Types)
            {
                if (type.SuperType != "domain")
                {
                    retStr += $"{Visit(type, indent + 1)} - {type.SuperType}{Environment.NewLine}";
                }
                else
                {
                    retStr += $"{Visit(type, indent + 1)}{Environment.NewLine}";
                }
            }
            retStr += $"{IndentStr(indent)}){Environment.NewLine}";
            return retStr;
        }
    }
}
