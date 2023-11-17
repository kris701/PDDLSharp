using PDDLSharp.Models.AST;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Expressions
{
    public class TypeExp : BaseNamedNode, IExp
    {
        // This is the primary supertype. It is needed in the code generation part
        public string SuperType { get; set; }
        // This is the set of alternative supertypes (inherited types)
        public HashSet<string> SuperTypes { get; set; }

        public TypeExp(ASTNode node, INode? parent, string name, string superType, HashSet<string> altTypes) : base(node, parent, name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>();
            foreach (var type in altTypes)
                SuperTypes.Add(type);
            SuperTypes.Add(SuperType);
        }

        public TypeExp(INode? parent, string name, string superType, HashSet<string> altTypes) : base(parent, name)
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

        public TypeExp(ASTNode node, INode? parent, string name, string superType) : base(node, parent, name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>() { SuperType };
        }

        public TypeExp(INode? parent, string name, string superType) : base(parent, name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>() { SuperType };
        }

        public TypeExp(string name, string superType) : base(name)
        {
            SuperType = superType;
            if (SuperType == "")
                SuperType = "object";
            SuperTypes = new HashSet<string>() { SuperType };
        }

        public TypeExp(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
            SuperType = "object";
            SuperTypes = new HashSet<string>() { SuperType };
        }

        public TypeExp(INode? parent, string name) : base(parent, name)
        {
            SuperType = "object";
            SuperTypes = new HashSet<string>() { SuperType };
        }

        public TypeExp(string name) : base(name)
        {
            SuperType = "object";
            SuperTypes = new HashSet<string>() { SuperType };
        }

        public bool IsTypeOf(string typeName)
        {
            if (typeName == "object")
                return true;
            if (typeName == Name)
                return true;
            return SuperTypes.Any(x => x == typeName);
        }

        public override bool Equals(object? obj)
        {
            if (obj is TypeExp other)
            {
                if (!base.Equals(other)) return false;
                if (!SuperType.Equals(other.SuperType)) return false;
                if (!EqualityHelper.AreListsEqual(SuperTypes, other.SuperTypes)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hash = SuperType.GetHashCode() + base.GetHashCode();
            if (SuperTypes != null)
                foreach (var type in SuperTypes)
                    hash *= type.GetHashCode();
            return hash;
        }

        public override TypeExp Copy(INode? newParent = null)
        {
            var newNode = new TypeExp(new ASTNode(Start, End, Line, "", ""), newParent, Name, SuperType);
            foreach (var superType in SuperTypes)
                newNode.SuperTypes.Add(superType);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
