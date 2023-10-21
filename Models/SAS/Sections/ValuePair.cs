namespace PDDLSharp.Models.SAS.Sections
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
    }
}
