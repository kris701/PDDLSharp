using PDDL.Models.AST;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDL.Tools;

namespace PDDL.Models.Domain
{
    public class DomainDecl : BaseWalkableNode, IDecl
    {
        public DomainNameDecl Name { get; set; }
        public RequirementsDecl Requirements { get; set; }
        public ExtendsDecl Extends { get; set; }
        public TimelessDecl Timeless { get; set; }
        public TypesDecl Types { get; set; }
        public ConstantsDecl Constants { get; set; }
        public PredicatesDecl Predicates { get; set; }
        public FunctionsDecl Functions { get; set; }
        public List<ActionDecl> Actions { get; set; }
        public List<DurativeActionDecl> DurativeActions { get; set; }
        public List<AxiomDecl> Axioms { get; set; }

        public DomainDecl(ASTNode node) : base(node, null)
        {
            Actions = new List<ActionDecl>();
            Axioms = new List<AxiomDecl>();
        }

        public bool ContainsType(string target)
        {
            if (target == "")
                return true;
            if (Types == null)
                return false;
            foreach(var type in Types.Types)
            {
                if (type.IsTypeOf(target))
                    return true;
            }
            return false;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            HashSet<INamedNode> res = new HashSet<INamedNode>();

            if (Name != null)
                res.AddRange(Name.FindNames(name));
            if (Requirements != null)
                res.AddRange(Requirements.FindNames(name));
            if (Extends != null)
                res.AddRange(Extends.FindNames(name));
            if (Timeless != null)
                res.AddRange(Timeless.FindNames(name));
            if (Types != null)
                res.AddRange(Types.FindNames(name));
            if (Constants != null)
                res.AddRange(Constants.FindNames(name));
            if (Predicates != null)
                res.AddRange(Predicates.FindNames(name));
            if (Functions != null)
                res.AddRange(Functions.FindNames(name));
            if (Actions != null)
                foreach (var act in Actions)
                    res.AddRange(act.FindNames(name));
            if (DurativeActions != null)
                foreach (var act in DurativeActions)
                    res.AddRange(act.FindNames(name));
            if (Axioms != null)
                foreach(var axi in Axioms)
                    res.AddRange(axi.FindNames(name));

            return res;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            if (Name != null)
                res.AddRange(Name.FindTypes<T>());
            if (Requirements != null)
                res.AddRange(Requirements.FindTypes<T>());
            if (Extends != null)
                res.AddRange(Extends.FindTypes<T>());
            if (Timeless != null)
                res.AddRange(Timeless.FindTypes<T>());
            if (Types != null)
                res.AddRange(Types.FindTypes<T>());
            if (Constants != null)
                res.AddRange(Constants.FindTypes<T>());
            if (Predicates != null)
                res.AddRange(Predicates.FindTypes<T>());
            if (Functions != null)
                res.AddRange(Functions.FindTypes<T>());
            if (Actions != null)
                foreach (var act in Actions)
                    res.AddRange(act.FindTypes<T>());
            if (DurativeActions != null)
                foreach (var act in DurativeActions)
                    res.AddRange(act.FindTypes<T>());
            if (Axioms != null)
                foreach (var axi in Axioms)
                    res.AddRange(axi.FindTypes<T>());

            return res;
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
                foreach(var act in Actions)
                    hash *= act.GetHashCode();
            if (DurativeActions != null)
                foreach (var act in DurativeActions)
                    hash *= act.GetHashCode();
            if (Axioms != null)
                foreach(var axi in Axioms)
                    hash *= axi.GetHashCode();

            return hash;
        }

        public override bool Equals(object? obj)
        {
            if (obj is DomainDecl exp)
                return exp.GetHashCode() == GetHashCode();
            return false;
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
                foreach(var act in DurativeActions)
                    yield return act;
            if (Axioms != null)
                foreach(var axi in Axioms)
                    yield return axi;
        }
    }
}
