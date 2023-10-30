namespace PDDLSharp.Models.FastDownward.SAS
{
    public interface ISASNode
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int Line { get; set; }
    }
}
