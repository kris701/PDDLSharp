namespace PDDLSharp.CodeGenerators.Visitors
{
    public partial class GeneratorVisitors
    {
        private bool _printType = false;
        private bool _readable = false;
        public GeneratorVisitors(bool readable)
        {
            _readable = readable;
        }

        internal string IndentStr(int by)
        {
            if (!_readable)
                return "";
            return new string('\t', by);
        }
    }
}
