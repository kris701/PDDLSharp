﻿using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDDLSharp.Tools;
using PDDLSharp.Models.Expressions;

namespace PDDLSharp.Models.Domain
{
    public class TypesDecl : BaseWalkableNode, IDecl
    {
        public List<TypeExp> Types { get; set; }

        public TypesDecl(ASTNode node, INode? parent, List<TypeExp> types) : base(node, parent)
        {
            Types = types;
        }

        public TypesDecl(INode? parent, List<TypeExp> types) : base(parent)
        {
            Types = types;
        }

        public override int GetHashCode()
        {
            var hash = base.GetHashCode();
            foreach (var type in Types)
                hash *= type.GetHashCode();
            return hash;
        }

        public override IEnumerator<INode> GetEnumerator()
        {
            return Types.GetEnumerator();
        }
    }
}
