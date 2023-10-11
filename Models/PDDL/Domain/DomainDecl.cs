﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class DomainDecl : BaseWalkableNode, IDecl
    {
        public DomainNameDecl? Name { get; set; }
        public RequirementsDecl? Requirements { get; set; }
        public ExtendsDecl? Extends { get; set; }
        public TimelessDecl? Timeless { get; set; }
        public TypesDecl? Types { get; set; }
        public ConstantsDecl? Constants { get; set; }
        public PredicatesDecl? Predicates { get; set; }
        public FunctionsDecl? Functions { get; set; }
        public List<ActionDecl> Actions { get; set; }
        public List<DurativeActionDecl> DurativeActions { get; set; }
        public List<AxiomDecl> Axioms { get; set; }
        public List<DerivedDecl> Deriveds { get; set; }

        public DomainDecl(ASTNode node) : base(node, null)
        {
            Actions = new List<ActionDecl>();
            Axioms = new List<AxiomDecl>();
            DurativeActions = new List<DurativeActionDecl>();
            Deriveds = new List<DerivedDecl>();
        }

        public DomainDecl() : base(null)
        {
            Actions = new List<ActionDecl>();
            Axioms = new List<AxiomDecl>();
            DurativeActions = new List<DurativeActionDecl>();
            Deriveds = new List<DerivedDecl>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();

            if (Name != null)
                hash *= Name.GetHashCode();
            if (Requirements != null)
                hash *= Requirements.GetHashCode();
            if (Extends != null)
                hash *= Extends.GetHashCode();
            if (Timeless != null)
                hash *= Timeless.GetHashCode();
            if (Types != null)
                hash *= Types.GetHashCode();
            if (Constants != null)
                hash *= Constants.GetHashCode();
            if (Predicates != null)
                hash *= Predicates.GetHashCode();
            if (Functions != null)
                hash *= Functions.GetHashCode();
            if (Actions != null)
                foreach (var act in Actions)
                    hash *= act.GetHashCode();
            if (DurativeActions != null)
                foreach (var act in DurativeActions)
                    hash *= act.GetHashCode();
            if (Axioms != null)
                foreach (var axi in Axioms)
                    hash *= axi.GetHashCode();
            if (Deriveds != null)
                foreach (var der in Deriveds)
                    hash *= der.GetHashCode();

            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            if (Name != null)
                yield return Name;
            if (Requirements != null) yield return Requirements;
            if (Extends != null) yield return Extends;
            if (Timeless != null) yield return Timeless;
            if (Types != null) yield return Types;
            if (Constants != null) yield return Constants;
            if (Predicates != null) yield return Predicates;
            if (Functions != null) yield return Functions;
            if (Actions != null)
                foreach (var act in Actions)
                    yield return act;
            if (DurativeActions != null)
                foreach (var act in DurativeActions)
                    yield return act;
            if (Axioms != null)
                foreach (var axi in Axioms)
                    yield return axi;
            if (Deriveds != null)
                foreach (var deri in Deriveds)
                    yield return deri;
        }

        public override DomainDecl Copy(INode? newParent = null)
        {
            var newNode = new DomainDecl(new ASTNode(Start, End, Line, "", ""));

            if (Name != null)
                newNode.Name = Name.Copy(newNode);
            if (Requirements != null)
                newNode.Requirements = Requirements.Copy(newNode);
            if (Extends != null)
                newNode.Extends = Extends.Copy(newNode);
            if (Timeless != null)
                newNode.Timeless = Timeless.Copy(newNode);
            if (Types != null)
                newNode.Types = Types.Copy(newNode);
            if (Constants != null)
                newNode.Constants = Constants.Copy(newNode);
            if (Predicates != null)
                newNode.Predicates = Predicates.Copy(newNode);
            if (Functions != null)
                newNode.Functions = Functions.Copy(newNode);
            foreach (var act in Actions)
                newNode.Actions.Add(act.Copy(newNode));
            foreach (var act in Axioms)
                newNode.Axioms.Add(act.Copy(newNode));
            foreach (var act in DurativeActions)
                newNode.DurativeActions.Add(act.Copy(newNode));
            foreach (var act in Deriveds)
                newNode.Deriveds.Add(act.Copy(newNode));

            return newNode;
        }
    }
}
