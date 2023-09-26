using Models.AST;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Models
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

        public override bool Equals(object obj)
        {
            if (obj is TypeExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            var result = new HashSet<INamedNode>();
            if (Name == name)
                result.Add(this);
            return result;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            return res;
        }
    }
}
