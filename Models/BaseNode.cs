using PDDL.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDL.Models
{
    public abstract class BaseNode : INode
    {
        public INode? Parent { get; }
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }

        public BaseNode(ASTNode node, INode? parent)
        {
            Line = -1;
            Start = -1;
            End = -1;
            if (node != null)
            {
                Line = node.Line;
                Start = node.Start;
                End = node.End;
            }
            Parent = parent;
        }

        public abstract HashSet<INamedNode> FindNames(string name);
        public abstract HashSet<T> FindTypes<T>();

        public override int GetHashCode()
        {
            return Start.GetHashCode() + End.GetHashCode() + Line.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;
            if (obj is not INode)
                return false;
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
