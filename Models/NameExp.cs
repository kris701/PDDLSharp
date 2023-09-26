using Models.AST;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools;

namespace Models
{
    public class NameExp : BaseNode, IExp, ICloneable, INamedNode
    {
        public string Name { get; set; }
        public TypeExp Type { get; set; }

        public NameExp(ASTNode node, INode parent, string name, TypeExp type) : base(node, parent)
        {
            Name = name;
            Type = type;
        }

        public NameExp(ASTNode node, INode parent, string name) : base(node, parent) 
        {
            Name = name;
            Type = new TypeExp(node, this, "");
        }

        public NameExp(ASTNode node, INode parent) : base(node, parent)
        {
            Name = "";
            Type = null;
        }

        public override string ToString()
        {
            if (Type == null || Type.Name == "")
                return $"({Name})";
            else
                return $"({Name} - {Type})";
        }

        public override int GetHashCode()
        {
            if (Type != null)
                return Name.GetHashCode() + base.GetHashCode() + Type.GetHashCode();
            return Name.GetHashCode() + base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NameExp exp)
            {
                return exp.GetHashCode() == GetHashCode();
            }
            return false;
        }

        public object Clone()
        {
            return new NameExp(new ASTNode(Start, End, Line), Parent, Name, Type);
        }

        public override HashSet<INamedNode> FindNames(string name)
        {
            var result = new HashSet<INamedNode>();
            if (Name == name)
                result.Add(this);
            if (Type != null)
                result.AddRange(Type.FindNames(name));
            return result;
        }

        public override HashSet<T> FindTypes<T>()
        {
            HashSet<T> res = new HashSet<T>();
            if (this is T v)
                res.Add(v);
            if (Type != null)
                res.AddRange(Type.FindTypes<T>());
            return res;
        }
    }
}
