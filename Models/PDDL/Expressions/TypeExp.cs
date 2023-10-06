using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class TypeExp : BaseNamedNode, IExp
    {
        // This is the primary supertype. It is needed in the code generation part
        public string SuperType { get; set; }
        // This is the set of alternative supertypes (inherited types)
        public HashSet<string> SuperTypes { get; set; }

        public TypeExp(ASTNode node, INode parent, string name, string superType, HashSet<string> altTypes) : base(node, parent, name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>();
            foreach (var type in altTypes)
                SuperTypes.Add(type);
            SuperTypes.Add(SuperType);
        }

        public TypeExp(INode parent, string name, string superType, HashSet<string> altTypes) : base(parent, name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>();
            foreach (var type in altTypes)
                SuperTypes.Add(type);
            SuperTypes.Add(SuperType);
        }

        public TypeExp(string name, string superType, HashSet<string> altTypes) : base(name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>();
            foreach (var type in altTypes)
                SuperTypes.Add(type);
            SuperTypes.Add(SuperType);
        }

        public TypeExp(ASTNode node, INode parent, string name, string superType) : base(node, parent, name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>();
            SuperTypes.Add(SuperType);
        }

        public TypeExp(INode parent, string name, string superType) : base(parent, name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>();
            SuperTypes.Add(SuperType);
        }

        public TypeExp(string name, string superType) : base(name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>();
            SuperTypes.Add(SuperType);
        }

        public TypeExp(ASTNode node, INode parent, string name) : base(node, parent, name)
        {
            SuperType = "object";
            SuperTypes = new HashSet<string>();
            SuperTypes.Add(SuperType);
        }

        public TypeExp(INode parent, string name) : base(parent, name)
        {
            SuperType = "object";
            SuperTypes = new HashSet<string>();
            SuperTypes.Add(SuperType);
        }

        public TypeExp(string name) : base(name)
        {
            SuperType = "object";
            SuperTypes = new HashSet<string>();
            SuperTypes.Add(SuperType);
        }

        public TypeExp(TypeExp other) : base(other.Name)
        {
            SuperTypes = new HashSet<string>();
            foreach (var superType in other.SuperTypes)
                SuperTypes.Add(superType);
            SuperType = other.SuperType;
        }

        public bool IsTypeOf(string typeName)
        {
            if (typeName == "")
                return true;
            if (typeName == Name)
                return true;
            return SuperTypes.Any(x => x == typeName);
        }

        public override int GetHashCode()
        {
            int hash = SuperType.GetHashCode() + base.GetHashCode();
            if (SuperTypes != null)
                foreach (var type in SuperTypes)
                    hash *= type.GetHashCode();
            return hash;
        }
    }
}
