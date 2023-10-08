﻿namespace PDDLSharp.Models.AST
{
    public class ASTNode
    {
        public int Line { get; set; }
        public int Start { get; set; }
        public int End { get; set; }

        public string OuterContent { get; set; }
        public string InnerContent { get; set; }
        public List<ASTNode> Children { get; set; }
        public int Count
        {
            get
            {
                int count = 0;
                foreach (var child in Children)
                    count += child.Count;
                return count + 1;
            }
        }

        public ASTNode()
        {
            Line = -1;
            Start = -1;
            End = -1;
            OuterContent = "";
            InnerContent = "";
            Children = new List<ASTNode>();
        }

        public ASTNode(int start, int end, int line, string outer, string inner, List<ASTNode> children)
        {
            Line = line;
            Start = start;
            End = end;
            OuterContent = outer;
            InnerContent = inner;
            Children = children;
        }

        public ASTNode(int start, int end, string outer, string inner)
        {
            Line = -1;
            Start = start;
            End = end;
            OuterContent = outer;
            InnerContent = inner;
            Children = new List<ASTNode>();
        }

        public ASTNode(int start, int end, int line, string outer, string inner)
        {
            Line = line;
            Start = start;
            End = end;
            OuterContent = outer;
            InnerContent = inner;
            Children = new List<ASTNode>();
        }

        public override string ToString()
        {
            return OuterContent;
        }
    }
}
