using PDDLSharp.Models.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.Simulators.StateSpace
{
    public class OperatorObject
    {
        public string Name {  get; set; }
        public string Type { get; set; }

        public OperatorObject(OperatorObject other) : this(other.Name, other.Type)
        {
        }

        public OperatorObject(string name) : this (name, "object")
        {
        }

        public OperatorObject(string name, string type)
        {
            Name = name.ToLower();
            Type = type.ToLower();
            if (Type == "")
                Type = "object";
        }

        public OperatorObject(NameExp nameExp) : this(nameExp.Name, nameExp.Type.Name)
        {
        }

        public override string? ToString()
        {
            if (Type == "object" || Type == "")
                return $"{Name}";
            return $"{Name} - {Type}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is OperatorObject op)
                return op.GetHashCode() == GetHashCode();
            return false;
        }

        public override int GetHashCode()
        {
            var hash = Name.GetHashCode();
            hash ^= Type.GetHashCode();
            return hash;
        }
    }
}
