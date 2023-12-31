﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL
{
    public abstract class BaseNamedNode : BaseNode, INamedNode
    {
        public string Name { get; set; }

        protected BaseNamedNode(ASTNode node, INode? parent, string name) : base(node, parent)
        {
            Name = name;
        }

        protected BaseNamedNode(INode? parent, string name) : base(parent)
        {
            Name = name;
        }

        protected BaseNamedNode(string name)
        {
            Name = name;
        }

        public override string? ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is BaseNamedNode other)
            {
                if (!base.Equals(other)) return false;
                if (Name != other.Name) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Name.GetHashCode();
        }
    }
}
