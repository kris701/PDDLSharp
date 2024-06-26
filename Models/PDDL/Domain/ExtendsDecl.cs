﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using PDDLSharp.Tools;

namespace PDDLSharp.Models.PDDL.Domain
{
    public class ExtendsDecl : BaseListableNode, IDecl
    {
        public List<NameExp> Extends { get; set; }

        public ExtendsDecl(ASTNode node, INode? parent, List<NameExp> extends) : base(node, parent)
        {
            Extends = extends;
        }

        public ExtendsDecl(INode? parent, List<NameExp> extends) : base(parent)
        {
            Extends = extends;
        }

        public ExtendsDecl(List<NameExp> extends) : base()
        {
            Extends = extends;
        }

        public ExtendsDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Extends = new List<NameExp>();
        }

        public ExtendsDecl(INode? parent) : base(parent)
        {
            Extends = new List<NameExp>();
        }

        public ExtendsDecl() : base()
        {
            Extends = new List<NameExp>();
        }

        public override bool Equals(object? obj)
        {
            if (obj is ExtendsDecl other)
            {
                if (!base.Equals(other)) return false;
                if (!EqualityHelper.AreListsEqualUnordered(Extends, other.Extends)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var extend in Extends)
                hash *= extend.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Extends.GetEnumerator();
        }

        public override ExtendsDecl Copy(INode? newParent = null)
        {
            var newNode = new ExtendsDecl(new ASTNode(Line, "", ""), newParent);
            newNode._metaInfo = new List<System.Reflection.PropertyInfo>(_metaInfo);
            foreach (var node in Extends)
                newNode.Extends.Add(node.Copy(newNode));
            newNode.IsHidden = IsHidden;
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (int i = 0; i < Extends.Count; i++)
            {
                if (Extends[i] == node && with is NameExp name)
                    Extends[i] = name;
            }
        }

        public override void Add(INode node)
        {
            if (node is NameExp exp)
                Extends.Add(exp);
        }

        public override void Remove(INode node)
        {
            if (node is NameExp exp)
                Extends.Remove(exp);
        }
    }
}
