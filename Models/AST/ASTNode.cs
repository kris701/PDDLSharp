using PDDLSharp.Tools;

namespace PDDLSharp.Models.AST
{
    public class ASTNode
    {
        public int Line { get; set; }

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
            OuterContent = "";
            InnerContent = "";
            Children = new List<ASTNode>();
        }

        public ASTNode(int line, string outer, string inner, List<ASTNode> children)
        {
            Line = line;
            OuterContent = outer;
            InnerContent = inner;
            Children = children;
        }

        public ASTNode(string outer, string inner)
        {
            Line = -1;
            OuterContent = outer;
            InnerContent = inner;
            Children = new List<ASTNode>();
        }

        public ASTNode(int line, string outer, string inner)
        {
            Line = line;
            OuterContent = outer;
            InnerContent = inner;
            Children = new List<ASTNode>();
        }

        public override string ToString()
        {
            return OuterContent;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ASTNode other)
            {
                if (Line != other.Line) return false;
                if (OuterContent != other.OuterContent) return false;
                if (InnerContent != other.InnerContent) return false;
                if (!EqualityHelper.AreListsEqual(Children, other.Children)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hash = Line ^ OuterContent.GetHashCode() ^ InnerContent.GetHashCode();
            foreach(var child in Children)
                hash ^= child.GetHashCode();
            return hash;
        }
    }
}
