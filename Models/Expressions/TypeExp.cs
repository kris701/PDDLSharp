using PDDL.Models.AST;
using PDDL.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Models.Expressions
{
    public class TypeExp : BaseNode, IExp, INamedNode
    {
        public string Name { get; set; }
        public HashSet<string> SuperTypes { get; set; }

        public TypeExp(ASTNode node, INode parent, string name, HashSet<string> types) : base(node, parent)
        {
            Name = name;
            SuperTypes = new HashSet<string>();
            foreach (var type in types)
                SuperTypes.Add(type);
        }

        public TypeExp(ASTNode node, INode parent, string name) : base(node, parent)
        {
            Name = name;
            SuperTypes = new HashSet<string>();
        }

        public bool IsTypeOf(string typeName)
        {
            if (typeName == "")
                return true;
            if (typeName == Name)
                return true;
            return SuperTypes.Any(x => x == typeName);
        }

        public override string ToString()
        {
            return Name;
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
