using PDDLSharp.Models.AST;
using PDDLSharp.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Models.Expressions
{
    public class TypeExp : BaseNode, IExp, INamedNode
    {
        public string Name { get; set; }
        // This is the primary supertype. It is needed in the code generation part
        public string SuperType { get; set; }
        // This is the set of alternative supertypes (inherited types)
        public HashSet<string> SuperTypes { get; set; }

        public TypeExp(ASTNode node, INode? parent, string name, string superType, HashSet<string> altTypes) : base(node, parent)
        {
            Name = name;
            SuperType = superType;
            SuperTypes = new HashSet<string>();
            foreach (var type in altTypes)
                SuperTypes.Add(type);
            SuperTypes.Add(superType);
        }

        public TypeExp(ASTNode node, INode? parent, string name, string superType) : base(node, parent)
        {
            Name = name;
            SuperTypes = new HashSet<string>();
            SuperType = superType;
            SuperTypes.Add(superType);
        }

        public TypeExp(ASTNode node, INode? parent, string name) : base(node, parent)
        {
            Name = name;
            SuperTypes = new HashSet<string>();
            SuperType = "";
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
            int hash = Name.GetHashCode() + base.GetHashCode();
            if (SuperTypes != null)
                foreach (var type in SuperTypes)
                    hash *= type.GetHashCode();
            return hash;
        }
    }
}
