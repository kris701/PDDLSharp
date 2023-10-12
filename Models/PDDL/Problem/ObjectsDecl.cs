﻿using PDDLSharp.Models.AST;
using PDDLSharp.Models.PDDL.Expressions;
using System;

namespace PDDLSharp.Models.PDDL.Problem
{
    public class ObjectsDecl : BaseWalkableNode, IDecl
    {
        public List<NameExp> Objs { get; set; }

        public ObjectsDecl(ASTNode node, INode? parent, List<NameExp> types) : base(node, parent)
        {
            Objs = types;
        }

        public ObjectsDecl(INode? parent, List<NameExp> types) : base(parent)
        {
            Objs = types;
        }

        public ObjectsDecl(List<NameExp> types) : base()
        {
            Objs = types;
        }

        public ObjectsDecl(ASTNode node, INode? parent) : base(node, parent)
        {
            Objs = new List<NameExp>();
        }

        public ObjectsDecl(INode? parent) : base(parent)
        {
            Objs = new List<NameExp>();
        }

        public ObjectsDecl() : base()
        {
            Objs = new List<NameExp>();
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var obj in Objs)
                hash *= obj.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Objs.GetEnumerator();
        }

        public override ObjectsDecl Copy(INode? newParent = null)
        {
            var newNode = new ObjectsDecl(new ASTNode(Start, End, Line, "", ""), newParent);
            foreach (var node in Objs)
                newNode.Objs.Add(node.Copy(newNode));
            return newNode;
        }

        public override void Replace(INode node, INode with)
        {
            for (var i = 0; i < Objs.Count; i++)
            {
                if (Objs[i] == node && with is NameExp exp)
                    Objs[i] = exp;
            }
        }
    }
}
