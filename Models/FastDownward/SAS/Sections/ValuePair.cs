using PDDLSharp.Tools;

namespace PDDLSharp.Models.FastDownward.SAS.Sections
{
    public class ValuePair
    {
        public int Left { get; set; }
        public int Right { get; set; }

        public ValuePair(int left, int right)
        {
            Left = left;
            Right = right;
        }

        public override string? ToString()
        {
            return $"{Left} {Right}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is ValuePair other)
            {
                if (Left != other.Left) return false;
                if (Right != other.Right) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() ^ Right.GetHashCode();
        }
    }
}
