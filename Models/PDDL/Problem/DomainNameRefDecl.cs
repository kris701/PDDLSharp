﻿using PDDLSharp.Models.AST;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class DomainNameRefDecl : BaseNamedNode, IDecl
    {

        public DomainNameRefDecl(ASTNode node, INode? parent, string name) : base(node, parent, name)
        {
        }

        public DomainNameRefDecl(INode? parent, string name) : base(parent, name)
        {
        }

        public DomainNameRefDecl(string name) : base(name)
        {
        }

        public override bool Equals(object? obj)
        {
            if (obj is DomainNameRefDecl other)
            {
                if (!base.Equals(other)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override DomainNameRefDecl Copy(INode? newParent = null)
        {
            var newNode = new DomainNameRefDecl(new ASTNode(Line, "", ""), newParent, Name);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            newNode.IsHidden = IsHidden;
            return newNode;
        }
    }
}
